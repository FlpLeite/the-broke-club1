<template>
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div class="flex justify-between items-center mb-6">
        <h1 class="text-2xl font-bold text-gray-900">Metas Financeiras</h1>
        <button @click="abrirFormNovaMeta" class="btn btn-primary">
          Nova Meta
        </button>
      </div>
  
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div
          v-for="meta in goalsStore.goalProgress"
          :key="meta.id"
          class="card"
        >
          <div class="p-6">
            <div class="flex justify-between items-start mb-4">
              <h3 class="text-lg font-semibold text-gray-900">{{ meta.titulo }}</h3>
              <div class="flex space-x-2">
                <button
                  @click="abrirFormEdicao(meta)"
                  class="text-primary-600 hover:text-primary-800"
                >
                  Editar
                </button>
                <button
                  @click="excluirMeta(meta.id)"
                  class="text-danger hover:text-red-700"
                >
                  Excluir
                </button>
              </div>
            </div>
  
            <div class="space-y-4">
              <div>
                <p class="text-sm text-gray-500">Categoria</p>
                <p class="font-medium">{{ meta.categoria }}</p>
              </div>
  
              <div>
                <p class="text-sm text-gray-500">Meta</p>
                <p class="font-medium">{{ formataMoeda(meta.valorMeta) }}</p>
              </div>
  
              <div>
                <p class="text-sm text-gray-500">Progresso</p>
                <div class="mt-1">
                  <div class="h-2 bg-gray-200 rounded-full">
                    <div
                      class="h-2 rounded-full transition-all duration-500"
                      :class="meta.progresso! >= 100 ? 'bg-success' : 'bg-primary-600'"
                      :style="{ width: `${Math.min(meta.progresso!, 100)}%` }"
                    ></div>
                  </div>
                  <div class="mt-2 flex justify-between text-sm">
                    <span>{{ formataMoeda(meta.valorAtual) }}</span>
                    <span class="text-gray-500">{{ meta.progresso!.toFixed(1) }}%</span>
                  </div>
                </div>
              </div>
  
              <div>
                <p class="text-sm text-gray-500">Prazo</p>
                <p class="font-medium">{{ formataData(meta.dataLimite) }}</p>
              </div>
  
              <div v-if="meta.descricao" class="text-sm text-gray-600">
                {{ meta.descricao }}
              </div>
  
              <button
                v-if="meta.progresso! < 100"
                @click="abrirModalProgresso(meta)"
                class="btn btn-primary w-full"
              >
                Adicionar Progresso
              </button>
              <div
                v-else
                class="bg-green-100 text-green-800 text-sm font-medium px-4 py-2 rounded-md text-center"
              >
                Meta Alcançada!
              </div>
            </div>
          </div>
        </div>
      </div>
  
      <div
        v-if="mostrarForm"
        class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center z-50 p-4"
      >
        <div class="bg-white rounded-lg shadow-xl max-w-md w-full p-6">
          <h2 class="text-xl font-semibold mb-4">
            {{ estaEditando ? 'Editar Meta' : 'Nova Meta' }}
          </h2>
  
          <form @submit.prevent="submeterForm" class="space-y-4">
            <div>
              <label class="label">Título</label>
              <input
                v-model="titulo"
                type="text"
                class="input"
                placeholder="Ex: Fundo de emergência"
                required
              />
            </div>
  
            <div>
              <label class="label">Categoria</label>
              <select v-model="categoria" class="input" required>
                <option value="">Selecione uma categoria</option>
                <option
                  v-for="cat in categoriasDisponiveis"
                  :key="cat"
                  :value="cat"
                >
                  {{ cat }}
                </option>
              </select>
            </div>
  
            <div>
              <label class="label">Valor da Meta</label>
              <input
                v-model="valorMeta"
                type="number"
                min="0"
                step="0.01"
                class="input"
                required
              />
            </div>
  
            <div>
              <label class="label">Prazo</label>
              <input
                v-model="dataLimite"
                type="date"
                class="input"
                required
              />
            </div>
  
            <div>
              <label class="label">Descrição (opcional)</label>
              <textarea
                v-model="descricao"
                class="input"
                rows="3"
                placeholder="Descreva sua meta..."
              />
            </div>
  
            <div class="flex justify-end space-x-3 mt-6">
              <button
                type="button"
                @click="mostrarForm = false"
                class="btn btn-secondary"
              >
                Cancelar
              </button>
              <button type="submit" class="btn btn-primary">
                {{ estaEditando ? 'Atualizar' : 'Criar' }} Meta
              </button>
            </div>
          </form>
        </div>
      </div>
  
      <div
        v-if="mostrarModalProgresso"
        class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center z-50 p-4"
      >
        <div class="bg-white rounded-lg shadow-xl max-w-md w-full p-6">
          <div class="flex justify-between items-center mb-6">
            <h2 class="text-xl font-semibold">Adicionar Progresso</h2>
            <button
              @click="mostrarModalProgresso = false"
              class="text-gray-400 hover:text-gray-500"
            >
              ✕
            </button>
          </div>
  
          <div v-if="metaAtual" class="mb-6">
            <p class="text-sm text-gray-500 mb-1">Meta</p>
            <p class="font-medium mb-4">{{ metaAtual.titulo }}</p>
  
            <p class="text-sm text-gray-500 mb-1">Progresso Atual</p>
            <p class="font-medium mb-4">
              {{ formataMoeda(metaAtual.valorAtual) }} de
              {{ formataMoeda(metaAtual.valorMeta) }}
            </p>
  
            <p class="text-sm text-gray-500 mb-1">Falta</p>
            <p class="font-medium mb-4">
              {{ formataMoeda(metaAtual.valorMeta - metaAtual.valorAtual) }}
            </p>
  
            <form @submit.prevent="submeterProgresso">
              <div class="mb-4">
                <label class="label">Valor a adicionar</label>
                <input
                  v-model="valorProgresso"
                  type="number"
                  min="0"
                  step="0.01"
                  :max="metaAtual.valorMeta - metaAtual.valorAtual"
                  class="input"
                  required
                />
              </div>
  
              <div class="flex justify-end space-x-3">
                <button
                  type="button"
                  @click="mostrarModalProgresso = false"
                  class="btn btn-secondary"
                >
                  Cancelar
                </button>
                <button
                  type="submit"
                  class="btn btn-primary"
                  :disabled="valorProgresso <= 0"
                >
                  Adicionar
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  </template>
  
  <script setup lang="ts">
  import { onMounted, ref } from 'vue'
  import { useGoalsStore, type Goal } from '../stores/goals'
  
  const goalsStore = useGoalsStore()
  
  onMounted(async () => {
    await goalsStore.loadGoals()
  })
  
  const mostrarForm = ref(false)
  const mostrarModalProgresso = ref(false)
  const estaEditando = ref(false)
  const metaAtual = ref<Goal | null>(null)
  const valorProgresso = ref(0)
  
  const titulo     = ref('')
  const categoria  = ref('')
  const valorMeta  = ref(0)
  const dataLimite = ref('')
  const descricao  = ref('')
  
  const categoriasDisponiveis = [
    'Emergência',
    'Aposentadoria',
    'Viagem',
    'Educação',
    'Casa própria',
    'Carro',
    'Casamento',
    'Outros'
  ]
  
  function resetForm() {
    titulo.value     = ''
    categoria.value  = ''
    valorMeta.value  = 0
    dataLimite.value = ''
    descricao.value  = ''
    estaEditando.value = false
    metaAtual.value    = null
  }
  
  function abrirFormNovaMeta() {
    resetForm()
    mostrarForm.value = true
  }
  
  function abrirFormEdicao(goal: Goal) {
    metaAtual.value    = goal
    titulo.value       = goal.titulo
    categoria.value    = goal.categoria
    valorMeta.value    = goal.valorMeta
    dataLimite.value   = goal.dataLimite
    descricao.value    = goal.descricao ?? ''
    estaEditando.value = true
    mostrarForm.value  = true
  }
  
  async function submeterForm() {
    if (
      !titulo.value ||
      !categoria.value ||
      !valorMeta.value ||
      !dataLimite.value
    ) {
      return
    }
  
    const payload = {
      titulo:     titulo.value,
      categoria:  categoria.value,
      valorMeta:  valorMeta.value,
      dataLimite: dataLimite.value,
      descricao:  descricao.value || undefined
    }
  
    if (estaEditando.value && metaAtual.value) {
      await goalsStore.updateGoal(metaAtual.value.id, payload)
    } else {
      await goalsStore.addGoal(payload)
    }
  
    mostrarForm.value = false
    resetForm()
  }

  async function excluirMeta(id: string) {
    if (confirm('Tem certeza que deseja excluir esta meta?')) {
      await goalsStore.deleteGoal(id)
    }
  }
  
  function formataMoeda(valor: number) {
    return new Intl.NumberFormat('pt-BR', {
      style:    'currency',
      currency: 'BRL'
    }).format(valor)
  }
  function formataData(dataStr: string) {
    return new Date(dataStr).toLocaleDateString('pt-BR', {
      year:  'numeric',
      month: 'short',
      day:   'numeric'
    })
  }
  
  function abrirModalProgresso(goal: Goal) {
    metaAtual.value      = goal
    valorProgresso.value = 0
    mostrarModalProgresso.value = true
  }
  async function submeterProgresso() {
    if (metaAtual.value && valorProgresso.value > 0) {
      await goalsStore.updateGoalProgress(metaAtual.value.id, valorProgresso.value)
      mostrarModalProgresso.value = false
      valorProgresso.value = 0
    }
  }
  </script>