<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useTransactionsStore } from '../stores/transactions'
import BalanceCard from '../components/dashboard/BalanceCard.vue'
import ExpensesChart from '../components/dashboard/ExpensesChart.vue'
import SavingsProgress from '../components/dashboard/SavingsProgress.vue'
import RecentTransactions from '../components/dashboard/RecentTransactions.vue'

const router = useRouter()
const authStore = useAuthStore()
const transactionsStore = useTransactionsStore()

const dateFrom = ref('')
const dateTo = ref('')
const quickPeriod = ref<'custom'|'month'|'quarter'>('custom')

watch(quickPeriod, period => {
  const now = new Date()
  if (period === 'month') {
    const start = new Date(now.getFullYear(), now.getMonth(), 1)
    const end   = new Date(now.getFullYear(), now.getMonth()+1, 0)
    dateFrom.value = start.toISOString().slice(0,10)
    dateTo.value   = end  .toISOString().slice(0,10)
  }
  else if (period === 'quarter') {
    const q = Math.floor(now.getMonth() / 3)
    const start = new Date(now.getFullYear(), q*3,     1)
    const end   = new Date(now.getFullYear(), q*3 + 3, 0)
    dateFrom.value = start.toISOString().slice(0,10)
    dateTo.value   = end  .toISOString().slice(0,10)
  }
  else {
    dateFrom.value = ''
    dateTo.value   = ''
  }
})

const filteredTransactions = computed(() =>
  transactionsStore.transactions.filter(tx => {
    return (!dateFrom.value || tx.date >= dateFrom.value)
        && (!dateTo.value   || tx.date <= dateTo.value)
  })
)

const resetFilters = () => {
  dateFrom.value = ''
  dateTo.value = ''
}

onMounted(async() => {
  if (!authStore.isAuthenticated) {
    router.push('/login')
  } else {
    await transactionsStore.loadTransactions()
  }
})
</script>

<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <div class="flex flex-col md:flex-row md:items-center md:justify-between mb-6">
      <h1 class="text-2xl font-bold text-gray-900">Painel Financeiro</h1>
      
      <div class="mt-4 md:mt-0 flex flex-wrap gap-4 items-end">
        <div>
          <label class="block text-sm font-medium text-gray-700">Período</label>
          <select v-model="quickPeriod" class="mt-1 block w-full rounded-md border-gray-300 shadow-sm sm:text-sm">
            <option value="custom">Personalizado</option>
            <option value="month">Mês Atual</option>
            <option value="quarter">Trimestre Atual</option>
          </select>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700">De</label>
          <input type="date" v-model="dateFrom"
            class="mt-1 block w-full rounded-md border-gray-300 shadow-sm sm:text-sm" />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700">Até</label>
          <input type="date" v-model="dateTo"
            class="mt-1 block w-full rounded-md border-gray-300 shadow-sm sm:text-sm" />
        </div>

        <button @click="resetFilters" 
          class="btn btn-secondary h-10 self-end ml-4">
          Limpar Filtros
        </button>
      </div>
    </div>
    
    <div class="grid grid-cols-1 gap-6 mb-6">
      <BalanceCard :transactions="filteredTransactions" />
    </div>
    
    <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
      <ExpensesChart :transactions="filteredTransactions" />
      <SavingsProgress :transactions="filteredTransactions" />
    </div>
    
    <div class="grid grid-cols-1 gap-6">
      <RecentTransactions :transactions="filteredTransactions" />
    </div>
  </div>
</template>