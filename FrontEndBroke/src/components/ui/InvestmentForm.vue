<script setup lang="ts">
import { ref, defineEmits, defineProps } from 'vue'
import { useInvestmentsStore, Investment } from '../../stores/investiments.ts'

const props = defineProps<{
  investment?: Investment
  isEditing?: boolean
}>()

const emit = defineEmits(['close'])
const investmentsStore = useInvestmentsStore()

const name = ref(props.investment?.name || '')
const type = ref(props.investment?.type || 'stocks')
const amount = ref(props.investment?.amount || 0)
const currentValue = ref(props.investment?.currentValue || 0)
const purchaseDate = ref(props.investment?.purchaseDate || new Date().toISOString().split('T')[0])
const broker = ref(props.investment?.broker || '')
const notes = ref(props.investment?.notes || '')

const errors = ref({
  name: '',
  amount: '',
  currentValue: '',
  purchaseDate: '',
  broker: ''
})

const validateForm = () => {
  let isValid = true
  errors.value = {
    name: '',
    amount: '',
    currentValue: '',
    purchaseDate: '',
    broker: ''
  }

  if (!name.value.trim()) {
    errors.value.name = 'Nome do investimento é obrigatório'
    isValid = false
  }

  if (!amount.value || amount.value <= 0) {
    errors.value.amount = 'Valor investido deve ser maior que 0'
    isValid = false
  }

  if (!currentValue.value || currentValue.value <= 0) {
    errors.value.currentValue = 'Valor atual deve ser maior que 0'
    isValid = false
  }

  if (!purchaseDate.value) {
    errors.value.purchaseDate = 'Data de compra é obrigatória'
    isValid = false
  }

  if (!broker.value.trim()) {
    errors.value.broker = 'Corretora é obrigatória'
    isValid = false
  }

  return isValid
}

const handleSubmit = async() => {
  if (!validateForm()) return

  const investmentData = {
    name: name.value,
    type: type.value as Investment['type'],
    amount: Number(amount.value),
    currentValue: Number(currentValue.value),
    purchaseDate: purchaseDate.value,
    broker: broker.value,
    notes: notes.value
  }

  try {
    if (props.isEditing && props.investment) {
      await investmentsStore.updateInvestment(props.investment.id, investmentData)
    } else {
      await investmentsStore.addInvestment(investmentData)
    }
    emit('close')
  } catch (error) {
    console.error('Erro ao salvar investimento:', error)
  }
}
</script>

<template>
  <div class="bg-white p-6 rounded-lg shadow-md">
    <h2 class="text-xl font-semibold mb-4">
      {{ isEditing ? 'Editar Investimento' : 'Novo Investimento' }}
    </h2>
    <form @submit.prevent="handleSubmit">
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label for="name" class="label">Nome do Investimento</label>
          <input
              id="name"
              v-model="name"
              type="text"
              class="input"
              :class="{ 'border-red-500': errors.name }"
              placeholder="ex: ITUB4, Tesouro Selic"
          />
          <p v-if="errors.name" class="mt-1 text-sm text-red-600">{{ errors.name }}</p>
        </div>

        <div>
          <label for="type" class="label">Tipo de Investimento</label>
          <select id="type" v-model="type" class="input">
            <option v-for="investmentType in investmentsStore.investmentTypes" :key="investmentType.value" :value="investmentType.value">
              {{ investmentType.label }}
            </option>
          </select>
        </div>

        <div>
          <label for="amount" class="label">Valor Investido</label>
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

        <div>
          <label for="currentValue" class="label">Valor Atual</label>
          <input
              id="currentValue"
              v-model="currentValue"
              type="number"
              step="0.01"
              class="input"
              :class="{ 'border-red-500': errors.currentValue }"
              placeholder="0,00"
          />
          <p v-if="errors.currentValue" class="mt-1 text-sm text-red-600">{{ errors.currentValue }}</p>
        </div>

        <div>
          <label for="purchaseDate" class="label">Data de Compra</label>
          <input
              id="purchaseDate"
              v-model="purchaseDate"
              type="date"
              class="input"
              :class="{ 'border-red-500': errors.purchaseDate }"
          />
          <p v-if="errors.purchaseDate" class="mt-1 text-sm text-red-600">{{ errors.purchaseDate }}</p>
        </div>

        <div>
          <label for="broker" class="label">Corretora</label>
          <input
              id="broker"
              v-model="broker"
              type="text"
              class="input"
              :class="{ 'border-red-500': errors.broker }"
              placeholder="ex: XP Investimentos, Rico"
          />
          <p v-if="errors.broker" class="mt-1 text-sm text-red-600">{{ errors.broker }}</p>
        </div>
      </div>

      <div class="mt-4">
        <label for="notes" class="label">Observações (opcional)</label>
        <textarea
            id="notes"
            v-model="notes"
            rows="3"
            class="input"
            placeholder="Adicione observações sobre este investimento..."
        ></textarea>
      </div>

      <div class="flex justify-end space-x-3 mt-6">
        <button type="button" @click="emit('close')" class="btn btn-secondary">
          Cancelar
        </button>
        <button type="submit" class="btn btn-primary">
          {{ isEditing ? 'Atualizar' : 'Adicionar' }} Investimento
        </button>
      </div>
    </form>
  </div>
</template>