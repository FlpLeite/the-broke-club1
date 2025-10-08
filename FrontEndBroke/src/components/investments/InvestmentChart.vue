<script setup lang="ts">
import { computed } from 'vue'
import { Doughnut } from 'vue-chartjs'
import { Chart as ChartJS, ArcElement, Tooltip, Legend, Title } from 'chart.js'
import { useInvestmentsStore } from '../../stores/investiments.ts'

ChartJS.register(ArcElement, Tooltip, Legend, Title)

const investmentsStore = useInvestmentsStore()

const chartData = computed(() => {
  const investmentsByType = investmentsStore.investmentsByType
  const labels = Object.keys(investmentsByType)
  const data = Object.values(investmentsByType).map(item => item.currentValue)

  const backgroundColors = labels.map((_, index) => {
    const hue = (index * 60) % 360
    return `hsl(${hue}, 70%, 60%)`
  })

  return {
    labels,
    datasets: [
      {
        data,
        backgroundColor: backgroundColors,
        borderWidth: 2,
        borderColor: '#ffffff'
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
      text: 'Distribuição dos Investimentos por Tipo',
      font: {
        size: 16
      }
    },
    tooltip: {
      callbacks: {
        label: (context: any) => {
          const value = new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
          }).format(context.raw)
          return `${context.label}: ${value}`
        }
      }
    }
  }
}
</script>

<template>
  <div class="card p-6">
    <div class="h-80">
      <Doughnut
          v-if="Object.keys(investmentsStore.investmentsByType).length > 0"
          :data="chartData"
          :options="chartOptions"
      />
      <div v-else class="h-full flex items-center justify-center">
        <p class="text-gray-500">Nenhum investimento registrado</p>
      </div>
    </div>
  </div>
</template>