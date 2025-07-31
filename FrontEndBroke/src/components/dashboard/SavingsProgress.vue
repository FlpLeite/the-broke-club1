<script setup lang="ts">
import { computed } from 'vue'
import { Line } from 'vue-chartjs'
import { Chart as ChartJS, CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend } from 'chart.js'
import type { Transaction } from '../../stores/transactions'

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend)

const props = defineProps<{
  transactions: Transaction[]
}>()

const savingsData = computed(() => {
  const transactions = [...props.transactions].sort((a, b) => 
    new Date(a.date).getTime() - new Date(b.date).getTime()
  )

  let balance = 0
  const data = transactions.map(t => {
    balance += t.type === 'income' ? t.amount : -t.amount
    return balance
  })

  const labels = transactions.map(t => 
    new Date(t.date).toLocaleDateString('pt-BR', { day: '2-digit', month: 'short' })
  )

  return {
    labels,
    datasets: [
      {
        label: 'Saldo Acumulado',
        data,
        borderColor: '#0ea5e9',
        backgroundColor: 'rgba(14, 165, 233, 0.1)',
        fill: true,
        tension: 0.4
      }
    ]
  }
})

const chartOptions = {
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
      text: 'Progresso da Economia',
      font: {
        size: 16
      }
    },
    tooltip: {
      callbacks: {
        label: (context: any) => {
          return `Saldo: ${new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
          }).format(context.raw)}`
        }
      }
    }
  },
  scales: {
    y: {
      ticks: {
        callback: (value: number) => {
          return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL',
            maximumFractionDigits: 0
          }).format(value)
        }
      }
    }
  }
}
</script>

<template>
  <div class="card p-6">
    <div class="h-80">
      <Line 
        v-if="transactions.length > 0"
        :data="savingsData" 
        :options="chartOptions" 
      />
      <div v-else class="h-full flex items-center justify-center">
        <p class="text-gray-500">Nenhuma transação registrada</p>
      </div>
    </div>
  </div>
</template>