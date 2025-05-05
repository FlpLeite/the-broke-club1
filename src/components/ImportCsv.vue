<template>
    <div class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white p-6 rounded-lg w-full max-w-lg">
        <h2 class="text-lg font-semibold mb-4">Importar Planilha CSV</h2>
        <input type="file" accept=".csv" @change="handleFileUpload" class="mb-4" />
  
        <div v-if="feedback.processing" class="mb-4">
          Processando... Aguarde.
        </div>
  
        <div v-else class="space-y-2 mb-4">
          <p>Total de linhas: {{ feedback.total }}</p>
          <p>Importadas com sucesso: {{ feedback.success }}</p>
          <div v-if="feedback.errors.length">
            <p class="text-red-600 font-semibold">Erros:</p>
            <ul class="list-disc pl-5 text-sm text-red-700">
              <li v-for="(err, i) in feedback.errors" :key="i">{{ err }}</li>
            </ul>
          </div>
        </div>
  
        <div class="flex justify-end space-x-2">
          <button @click="onCancel" class="btn btn-secondary">Cancelar</button>
          <button @click="onClose"   class="btn btn-primary">Fechar</button>
        </div>
      </div>
    </div>
  </template>
  
  <script setup lang="ts">
  import { reactive } from 'vue'
  import Papa from 'papaparse'
  import type { Transaction } from '../stores/transactions'
  import { useTransactionsStore } from '../stores/transactions'
  import { useAuthStore } from '../stores/auth'
  
  const transactionsStore = useTransactionsStore()
  const authStore         = useAuthStore()
  const emit              = defineEmits(['close'])
  
  const feedback = reactive({
    total:      0,
    success:    0,
    errors:    [] as string[],
    processing: false
  })
  
  function mapTipoCsv(tipoCsv: string): 'income' | 'expense' {
    return tipoCsv.trim().toLowerCase() === 'receita' ? 'income' : 'expense'
  }
  
  function normalizeKey(k: string): string {
    return k
      .normalize('NFD')                   
      .replace(/[\u0300-\u036f]/g, '')   
      .trim()                             
      .toLowerCase()                      
  }
  
  async function handleFileUpload(event: Event) {
    const input = event.target as HTMLInputElement
    if (!input.files?.length) return
    const file = input.files[0]
  
    // reset feedback
    feedback.processing = true
    feedback.total      = 0
    feedback.success    = 0
    feedback.errors     = []
  
    Papa.parse(file, {
      header:         true,
      skipEmptyLines: true,
      complete: async (results) => {
        const rawFields = results.meta.fields || []
        const headerMap = (rawFields as string[]).reduce<Record<string,string>>((acc, rawKey) => {
          acc[ normalizeKey(rawKey) ] = rawKey
          return acc
        }, {})
  
        const rows = results.data as Record<string,string>[]
        feedback.total = rows.length
  
        for (let i = 0; i < rows.length; i++) {
          const row = rows[i]
          try {
            // procura as colunas normalizadas
            const tipoKey = headerMap['tipo']
            const catKey  = headerMap['categoria']
            const valKey  = headerMap['valor']
            const dtKey   = headerMap['datatransacao']
  
            const tipoCsv      = tipoKey ? row[tipoKey] : ''
            const categoriaCsv = catKey  ? row[catKey]   : ''
            const valorCsv     = valKey  ? row[valKey]   : ''
            const dataCsv      = dtKey   ? row[dtKey]    : ''
  
            if (!tipoCsv || !categoriaCsv || !valorCsv || !dataCsv) {
              throw new Error(`Linha ${i+1}: faltou alguma coluna obrigatória`)
            }
  
            const payload: Omit<Transaction,'id'> & { idUsuario: number } = {
              type:        mapTipoCsv(tipoCsv),
              category:    categoriaCsv,
              amount:      parseFloat(valorCsv.replace(',', '.')),
              date:        dataCsv.slice(0,10),
              description: '',
              idUsuario:   authStore.user!.idUsuario
            }
  
            await transactionsStore.addTransaction(payload)
            feedback.success++
          }
          catch(err: any) {
            feedback.errors.push(err.message)
          }
        }
  
        feedback.processing = false
      },
      error: (err) => {
        feedback.processing = false
        feedback.errors.push(`Falha ao ler CSV: ${err.message}`)
      }
    })
  }
  
  function onCancel() {
    emit('close')
  }
  function onClose() {
    emit('close')
  }
  </script>
  