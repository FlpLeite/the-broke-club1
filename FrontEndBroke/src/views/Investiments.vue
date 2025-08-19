<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useTransactionsStore } from '../stores/transactions'
import { useInvestmentsStore } from '../stores/investments'
import { useAuthStore } from '../stores/auth'
import InvestmentCard from '../components/InvestmentCard.vue'

const transactionsStore = useTransactionsStore()
const investmentsStore = useInvestmentsStore()
const authStore = useAuthStore()
const dateFrom = ref('')
const dateTo = ref('')

const open = ref(false)
const dlg = ref<HTMLDialogElement | null>(null)
const ticker = ref('')
const nome = ref('')

onMounted(async () => {
  if (authStore.user) {
    await investmentsStore.loadCards(authStore.user.idUsuario)
    await investmentsStore.refreshAtivos(authStore.user.idUsuario)
  }
})

watch(open, v => v && dlg.value?.showModal())

async function submit() {
  if (authStore.user) {
    await investmentsStore.addAtivo(authStore.user.idUsuario, ticker.value.trim(), nome.value.trim())
    ticker.value = ''
    nome.value = ''
    open.value = false
    dlg.value?.close()
  }
}

const filteredTransactions = computed(() => {
  return transactionsStore.transactions
      .filter(transaction => {
            const isInvestment = transaction.category === 'Investimentos'
            let matchesDateRange = true
            if (dateFrom.value) {
              matchesDateRange = matchesDateRange && transaction.date >= dateFrom.value
      }
        if (dateTo.value) {
          matchesDateRange = matchesDateRange && transaction.date <= dateTo.value
      }
        return isInvestment && matchesDateRange
      })
      .sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
})

const totalInvestments = computed(() => {
  return filteredTransactions.value.reduce((sum, t) => sum + t.amount, 0)
})

const formatCurrency = (value: number) => {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL'
  }).format(value)
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('pt-BR', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

const resetFilters = () => {
  dateFrom.value = ''
  dateTo.value = ''
}
</script>

<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <div class="flex flex-col md:flex-row md:items-center md:justify-between mb-6">
      <h1 class="text-2xl font-bold text-gray-900 mb-4 md:mb-0">Investimentos</h1>

      <div class="flex flex-wrap gap-4">
        <div class="flex gap-4">
          <div>
            <label for="dateFrom" class="label">De</label>
            <input
              type="date"
              id="dateFrom"
              v-model="dateFrom"
              class="input"
            />
          </div>
          <div>
            <label for="dateTo" class="label">Até</label>
            <input
              type="date"
              id="dateTo"
              v-model="dateTo"
              class="input"
            />
          </div>
        </div>

        <button @click="resetFilters" class="btn btn-secondary self-end">
          Limpar Filtros
        </button>
      </div>
    </div>

    <!-- Resumo dos Investimentos -->
    <div class="bg-white rounded-lg shadow-md p-6 mb-6">
      <h2 class="text-lg font-semibold text-gray-700 mb-4">Total Investido</h2>
      <div class="bg-blue-50 p-4 rounded-lg">
        <p class="text-sm text-gray-500 mb-1">Valor Total em Investimentos</p>
        <p class="text-2xl font-bold text-primary-600">
          {{ formatCurrency(totalInvestments) }}
        </p>
      </div>
    </div>

    <!-- Cards de investimentos -->
    <div class="bg-white rounded-lg shadow-md p-6 mb-6">
      <div class="flex justify-between items-center mb-4">
        <h2 class="text-lg font-semibold text-gray-700">Meus Investimentos</h2>
        <button class="btn btn-primary" @click="open = true">Novo ativo</button>
      </div>

      <div v-if="investmentsStore.loading" class="text-gray-500">Carregando...</div>
      <div v-else-if="investmentsStore.error" class="text-red-500">{{ investmentsStore.error }}</div>
      <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
        <InvestmentCard v-for="c in investmentsStore.cards" :key="c.ativoId" :card="c" />
        <div v-if="investmentsStore.cards.length === 0" class="text-gray-500 col-span-full text-center">
          Nenhum ativo cadastrado
        </div>
      </div>

      <dialog ref="dlg" class="rounded-2xl p-0 w-full max-w-md" :open="open" @close="open=false">
        <form class="p-4 space-y-3" @submit.prevent="submit">
          <h2 class="text-lg font-medium">Novo ativo</h2>
          <div class="space-y-1">
            <label class="text-sm">Ticker (ex: PETR4.SA)</label>
            <input v-model="ticker" class="w-full rounded-xl border px-3 py-2 bg-white/5" required />
          </div>
          <div class="space-y-1">
            <label class="text-sm">Nome</label>
            <input v-model="nome" class="w-full rounded-xl border px-3 py-2 bg-white/5" required />
          </div>
          <div class="flex gap-2 justify-end pt-2">
            <button type="button" class="px-3 py-2 rounded-xl border" @click="open=false">Cancelar</button>
            <button class="px-3 py-2 rounded-xl bg-blue-600 text-white">Salvar</button>
          </div>
        </form>
      </dialog>
    </div>

    <!-- Lista de Investimentos -->
    <div class="bg-white rounded-lg shadow-md overflow-hidden">
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Data
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Descrição
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Categoria
              </th>
              <th scope="col" class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                Valor
              </th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="transaction in filteredTransactions" :key="transaction.id">
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ formatDate(transaction.date) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                {{ transaction.description }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ transaction.category }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-right font-medium text-primary-600">
                {{ formatCurrency(transaction.amount) }}
              </td>
            </tr>
            <tr v-if="filteredTransactions.length === 0">
              <td colspan="4" class="px-6 py-4 text-center text-sm text-gray-500">
                Nenhum investimento encontrado
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>