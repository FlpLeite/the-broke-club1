<script setup lang="ts">
import { ref, defineEmits, defineProps } from 'vue'
import { useTransactionsStore, Transaction } from '../../stores/transactions'
import { useAuthStore } from '../../stores/auth'

const authStore = useAuthStore()

const props = defineProps<{
  transaction?: Transaction
  isEditing?: boolean
}>()

const emit = defineEmits(['close'])
const transactionsStore = useTransactionsStore()

const date = ref(props.transaction?.date || new Date().toISOString().split('T')[0])
const description = ref(props.transaction?.description || '')
const category = ref(props.transaction?.category || 'Receita')
const amount = ref(props.transaction?.amount || 0)
const type = ref(props.transaction?.type || 'income')

const errors = ref({
  date: '',
  description: '',
  amount: ''
})

const validateForm = () => {
  let isValid = true
  errors.value = {
    date: '',
    description: '',
    amount: ''
  }

  if (!date.value) {
    errors.value.date = 'Data é obrigatória'
    isValid = false
  }

  if (!description.value.trim()) {
    errors.value.description = 'Descrição é obrigatória'
    isValid = false
  }

  if (!amount.value || amount.value <= 0) {
    errors.value.amount = 'Valor deve ser maior que 0'
    isValid = false
  }

  return isValid
}

const handleSubmit = async () => {
  if (!validateForm()) return

  const user = authStore.user
  if (!user) {
    throw new Error('Usuário não autenticado.')
  }

  const transactionData = {
    date:        date.value,
    description: description.value,
    category:    category.value,
    amount:      Number(amount.value),
    type:        type.value as 'income' | 'expense'
  }

  if (props.isEditing && props.transaction) {
    await transactionsStore.updateTransaction(props.transaction.id, transactionData)
  } else {
    await transactionsStore.addTransaction({
      ...transactionData,
      idUsuario: user.idUsuario
    })
  }

  emit('close')
}


</script>

<template>
 <div class="bg-white p-6 rounded-lg shadow-md">
    <h2 class="text-xl font-semibold mb-4">
      {{ isEditing ? 'Editar Transação' : 'Nova Transação' }}
    </h2>
    <form @submit.prevent="handleSubmit">
      <div class="mb-4">
        <label for="date" class="label">Data</label>
        <input
          id="date"
          v-model="date"
          type="date"
          class="input"
          :class="{ 'border-red-500': errors.date }"
        />
        <p v-if="errors.date" class="mt-1 text-sm text-red-600">{{ errors.date }}</p>
      </div>

      <div class="mb-4">
        <label for="description" class="label">Descrição</label>
        <input
          id="description"
          v-model="description"
          type="text"
          class="input"
          :class="{ 'border-red-500': errors.description }"
          placeholder="ex: Salário, Aluguel, Supermercado"
        />
        <p v-if="errors.description" class="mt-1 text-sm text-red-600">{{ errors.description }}</p>
      </div>

      <div class="mb-4">
        <label for="type" class="label">Tipo</label>
        <select id="type" v-model="type" class="input">
          <option value="income">Receita</option>
          <option value="expense">Despesa</option>
        </select>
      </div>

      <div class="mb-4">
        <label for="category" class="label">Categoria</label>
        <select id="category" v-model="category" class="input">
          <option v-for="cat in transactionsStore.categories" :key="cat" :value="cat">
            {{ cat }}
          </option>
        </select>
      </div>

      <div class="mb-6">
        <label for="amount" class="label">Valor</label>
        <input
          id="amount"
          v-model="amount"
          type="number"
          step="0.01"
          class="input"
          :class="{ 'border-red-500': errors.amount }"
          placeholder="0,00"
        />
        <p v-if="errors.amount" class="mt-1 text-sm text-red-600">{{ errors.amount }}</p>
      </div>

      <div class="flex justify-end space-x-3">
        <button type="button" @click="emit('close')" class="btn btn-secondary">
          Cancelar
        </button>
        <button type="submit" class="btn btn-primary">
          {{ isEditing ? 'Atualizar' : 'Adicionar' }} Transação
        </button>
      </div>
    </form>
  </div>
</template>