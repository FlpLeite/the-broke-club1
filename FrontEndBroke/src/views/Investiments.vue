<script setup lang="ts">
import { ref, computed } from 'vue'
import { useTransactionsStore } from '../stores/transactions'
import { Pie } from 'vue-chartjs'
import { Chart as ChartJS, ArcElement, Tooltip, Legend, Title } from 'chart.js'

ChartJS.register(ArcElement, Tooltip, Legend, Title)

const transactionsStore = useTransactionsStore()
const dateFrom = ref('')
const dateTo = ref('')

const filteredTransactions = computed(() => {
  return transactionsStore.transactions.filter(transaction => {
    const isInvestment = transaction.category === 'Investimentos'
    let matchesDateRange = true
    if (dateFrom.value) {
      matchesDateRange = matchesDateRange && transaction.date >= dateFrom.value
    }
    if (dateTo.value) {
      matchesDateRange = matchesDateRange && transaction.date <= dateTo.value
    }
    return isInvestment && matchesDateRange
  }).sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
})

const totalInvestments = computed(() => {
  return filteredTransactions.value.reduce((sum, t) => sum + t.amount, 0)
})

const investmentsByCategory = computed(() => {
  const result: Record<string, number> = {}
  
  filteredTransactions.value.forEach(t => {
    if (!result[t.category]) {
      result[t.category] = 0
    }
    result[t.category] += t.amount
  })
  
  return result
})

const chartData = computed(() => {
  const labels = Object.keys(investmentsByCategory.value)
  const data = Object.values(investmentsByCategory.value)
  
  const backgroundColors = labels.map((_, index) => {
    const hue = (index * 137) % 360
    return `hsl(${hue}, 70%, 60%)`
  })
  
  return {
    labels,
    datasets: [
      {
        data,
        backgroundColor: backgroundColors,
        borderWidth: 1
      }
    ]
  }
})

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'right' as const,
      labels: {
        font: {
          size: 12
        }
      }
    },
    title: {
      display: true,
      text: 'Distribuição dos Investimentos',
      font: {
        size: 16
      }
    }
  }
}

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

    <!-- Gráfico de Distribuição -->
    <div class="bg-white rounded-lg shadow-md p-6 mb-6">
      <div class="h-80">
        <Pie 
          v-if="Object.keys(investmentsByCategory).length > 0"
          :data="chartData" 
          :options="chartOptions" 
        />
        <div v-else class="h-full flex items-center justify-center">
          <p class="text-gray-500">Nenhum investimento registrado</p>
        </div>
      </div>
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