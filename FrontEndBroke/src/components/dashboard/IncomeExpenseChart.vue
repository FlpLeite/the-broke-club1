<script setup lang="ts">
import { computed } from 'vue'
import { Line } from 'vue-chartjs'
import { Chart as ChartJS, CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend } from 'chart.js'
import type { Transaction } from '../../stores/transactions'

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend)

const props = defineProps<{
  transactions: Transaction[]
}>()

let dateKeys: string[] = []
let detailsByDate: Record<string, { incomes: Transaction[]; expenses: Transaction[] }> = {}

const chartData = computed(() => {
  const transactions = [...props.transactions].sort((a, b) =>
      new Date(a.date).getTime() - new Date(b.date).getTime()
  )

  const map: Record<string, { incomes: Transaction[]; expenses: Transaction[] }> = {}
  transactions.forEach(t => {
    if (!map[t.date]) map[t.date] = { incomes: [], expenses: [] }
    if (t.type === 'income') map[t.date].incomes.push(t)
    else map[t.date].expenses.push(t)
  })

  const keys = Object.keys(map).sort(
      (a, b) => new Date(a).getTime() - new Date(b).getTime()
  )

  let incomeTotal = 0
  let expenseTotal = 0
  const incomeData: number[] = []
  const expenseData: number[] = []
  keys.forEach(d => {
    const dayIncome = map[d].incomes.reduce((sum, t) => sum + t.amount, 0)
    const dayExpense = map[d].expenses.reduce((sum, t) => sum + t.amount, 0)
    incomeTotal += dayIncome
    expenseTotal += dayExpense
    incomeData.push(incomeTotal)
    expenseData.push(expenseTotal)
  })

  dateKeys = keys
  detailsByDate = map

  const formattedLabels = keys.map(d =>
      new Date(d).toLocaleDateString('pt-BR', { day: '2-digit', month: 'short' })
  )

  return {
    labels: formattedLabels,
    datasets: [
      {
        label: 'Receitas',
        data: incomeData,
        borderColor: '#22c55e',
        backgroundColor: 'rgba(34, 197, 94, 0.1)',
        fill: false,
        tension: 0.4
      },
      {
        label: 'Despesas',
        data: expenseData,
        borderColor: '#ef4444',
        backgroundColor: 'rgba(239, 68, 68, 0.1)',
        fill: false,
        tension: 0.4
      }
    ]
  }
})

const formatter = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL'
})

const chartOptions = computed(() => ({
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'top' as const,
      labels: {
        font: {
          size: 12
        }
      }
    },
    title: {
      display: true,
      text: 'Receitas vs Despesas',
      font: {
        size: 16
      }
    },
    tooltip: {
      callbacks: {
        label: (context: any) => {
          const dateKey = dateKeys[context.dataIndex]
          const type = context.dataset.label === 'Receitas' ? 'incomes' : 'expenses'
          const transactions = detailsByDate[dateKey]?.[type] || []
          const header = `${context.dataset.label}: ${formatter.format(context.raw)}`
          if (transactions.length === 0) return header
          const lines = transactions.map(t =>
              `${t.description || t.category}: ${formatter.format(t.amount)}`
          )
          return [header, ...lines]
        }
      }
    }
  },
  scales: {
    y: {
      ticks: {
        callback: (value: string | number) => {
          return formatter.format(Number(value))
        }
      }
    }
  }
}))
</script>

<template>
  <div class="card p-6">
    <div class="h-80">
      <Line
          v-if="transactions.length > 0"
          :data="chartData"
          :options="chartOptions"
      />
      <div v-else class="h-full flex items-center justify-center">
        <p class="text-gray-500">Nenhuma transação registrada</p>
      </div>
    </div>
  </div>
</template>