<script setup lang="ts">
import { computed } from 'vue'
import { Pie } from 'vue-chartjs'
import { Chart as ChartJS, ArcElement, Tooltip, Legend, Title } from 'chart.js'
import type { Transaction } from '../../stores/transactions'

ChartJS.register(ArcElement, Tooltip, Legend, Title)

const props = defineProps<{
  transactions: Transaction[]
}>()

const expensesByCategory = computed(() => {
  const result: Record<string, number> = {}
  
  props.transactions
    .filter(t => t.type === 'expense')
    .forEach(t => {
      if (!result[t.category]) {
        result[t.category] = 0
      }
      result[t.category] += t.amount
    })
    
  return result
})

const chartData = computed(() => {
  const labels = Object.keys(expensesByCategory.value)
  const data = Object.values(expensesByCategory.value)
  
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
        boxWidth: 15,
        font: {
          size: 12
        }
      }
    },
    title: {
      display: true,
      text: 'Despesas por Categoria',
      font: {
        size: 16
      }
    }
  }
}
</script>

<template>
  <div class="card p-6">
    <div class="h-80">
      <Pie 
        v-if="Object.keys(expensesByCategory).length > 0"
        :data="chartData" 
        :options="chartOptions" 
      />
      <div v-else class="h-full flex items-center justify-center">
        <p class="text-gray-500">Nenhuma despesa registrada</p>
      </div>
    </div>
  </div>
</template>