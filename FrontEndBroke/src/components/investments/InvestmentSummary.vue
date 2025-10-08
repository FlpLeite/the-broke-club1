<script setup lang="ts">
import { computed } from 'vue'
import { useInvestmentsStore } from '../../stores/investiments.ts'

const investmentsStore = useInvestmentsStore()

const formatCurrency = (value: number) => {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL'
  }).format(value)
}

const returnClass = computed(() => {
  return investmentsStore.totalReturn >= 0 ? 'text-success' : 'text-danger'
})

const returnIcon = computed(() => {
  return investmentsStore.totalReturn >= 0 ? '↗' : '↘'
})
</script>

<template>
  <div class="card p-6">
    <h2 class="text-lg font-semibold text-gray-700 mb-4">Resumo dos Investimentos</h2>
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
      <div class="bg-blue-50 p-4 rounded-lg">
        <p class="text-sm text-gray-500 mb-1">Total Investido</p>
        <p class="text-xl font-bold text-primary-600">
          {{ formatCurrency(investmentsStore.totalInvested) }}
        </p>
      </div>
      <div class="bg-purple-50 p-4 rounded-lg">
        <p class="text-sm text-gray-500 mb-1">Valor Atual</p>
        <p class="text-xl font-bold text-purple-600">
          {{ formatCurrency(investmentsStore.totalCurrentValue) }}
        </p>
      </div>
      <div class="bg-gray-50 p-4 rounded-lg">
        <p class="text-sm text-gray-500 mb-1">Retorno</p>
        <p class="text-xl font-bold flex items-center" :class="returnClass">
          <span class="mr-1">{{ returnIcon }}</span>
          {{ formatCurrency(investmentsStore.totalReturn) }}
        </p>
      </div>
      <div class="bg-yellow-50 p-4 rounded-lg">
        <p class="text-sm text-gray-500 mb-1">Rentabilidade</p>
        <p class="text-xl font-bold flex items-center" :class="returnClass">
          <span class="mr-1">{{ returnIcon }}</span>
          {{ investmentsStore.totalReturnPercentage.toFixed(2) }}%
        </p>
      </div>
    </div>
  </div>
</template>