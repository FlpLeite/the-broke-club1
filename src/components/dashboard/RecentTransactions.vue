<script setup lang="ts">
import { computed } from 'vue'
import type { Transaction } from '../../stores/transactions'

const props = defineProps<{
  transactions: Transaction[]
}>()

const recentTransactions = computed(() => {
  return [...props.transactions]
    .sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
    .slice(0, 5)
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
</script>

<template>
  <div class="card p-6">
    <div class="flex justify-between items-center mb-4">
      <h2 class="text-lg font-semibold text-gray-700">Transações Recentes</h2>
      <router-link to="/transactions" class="text-sm text-primary-600 hover:text-primary-700">
        Ver Todas
      </router-link>
    </div>
    
    <div class="overflow-hidden">
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
          <tr v-for="transaction in recentTransactions" :key="transaction.id">
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ formatDate(transaction.date) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
              {{ transaction.description }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ transaction.category }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-right" 
                :class="transaction.type === 'income' ? 'text-success' : 'text-danger'">
              {{ transaction.type === 'income' ? '+' : '-' }} {{ formatCurrency(transaction.amount) }}
            </td>
          </tr>
          <tr v-if="recentTransactions.length === 0">
            <td colspan="4" class="px-6 py-4 text-center text-sm text-gray-500">
              Nenhuma transação encontrada
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>