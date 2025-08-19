<script setup lang="ts">
import { ref, defineEmits, defineProps, watch, onMounted } from 'vue'
import { useTransactionsStore, Transaction } from '../../stores/transactions'
import { useAuthStore } from '../../stores/auth'
import { useRouter } from 'vue-router'
import { useInvestmentsStore } from '../../stores/investments'

const authStore         = useAuthStore()
const transactionsStore = useTransactionsStore()
const investmentsStore  = useInvestmentsStore()
const router            = useRouter()

const props = defineProps<{
  transaction?: Transaction
  isEditing?: boolean
}>()

const emit = defineEmits(['close'])

// campos do formulário
const date        = ref(props.transaction?.date || new Date().toISOString().split('T')[0])
const description = ref(props.transaction?.description || '')
const category    = ref(props.transaction?.category || transactionsStore.categories[0])
const amount      = ref(props.transaction?.amount || 0)
const type        = ref<'income' | 'expense' | 'investment'>(props.transaction?.type || 'income')

// erros de validação
const errors = ref({ date: '', description: '', amount: '' })

function validateForm() {
  let valid = true
  errors.value = { date: '', description: '', amount: '' }

  if (!date.value) {
    errors.value.date = 'Data é obrigatória'
    valid = false
  }
  if (!description.value.trim()) {
    errors.value.description = 'Descrição é obrigatória'
    valid = false
  }
  if (!amount.value || amount.value <= 0) {
    errors.value.amount = 'Valor deve ser maior que 0'
    valid = false
  }
  return valid
}

async function handleSubmit() {
  if (!validateForm()) return

  // se não estiver logado, redireciona
  if (!authStore.user) {
    await router.push({ name: 'login' })
    return
  }

  // converte "investment" → "expense" para a API
  const apiType = type.value === 'investment' ? 'expense' : type.value

  // monta o payload com string|number puros, não refs
  const payload = {
    date:        date.value,
    description: description.value,
    category:    category.value,
    amount:      Number(amount.value),
    type:        apiType as 'income' | 'expense'
  }

  if (props.isEditing && props.transaction) {
    await transactionsStore.updateTransaction(props.transaction.id, payload)
  } else {
    await transactionsStore.addTransaction({
      ...payload,
      idUsuario: authStore.user.idUsuario
    })
  }

  emit('close')
}

onMounted(async () => {
  if (authStore.user) {
    await investmentsStore.refreshAtivos(authStore.user.idUsuario)
  }
})

watch(type, async (v) => {
  if (v === 'investment' && authStore.user) {
    await investmentsStore.refreshAtivos(authStore.user.idUsuario)
    description.value = ''
  }
})
</script>

<template>
  <div class="bg-white p-6 rounded-lg shadow-md max-w-md mx-auto">
    <h2 class="text-xl font-semibold mb-6">
      {{ props.isEditing ? 'Editar Transação' : 'Nova Transação' }}
    </h2>
    <form @submit.prevent="handleSubmit">
      <div class="radio-input grid grid-cols-3 gap-6 justify-items-center mb-6">
        <label class="flex flex-col items-center cursor-pointer">
          <input
            type="radio"
            name="tipo"
            value="income"
            v-model="type"
            class="input green"
          />
          <span class="mt-2 text-sm font-medium">Receita</span>
        </label>
        <label class="flex flex-col items-center cursor-pointer">
          <input
            type="radio"
            name="tipo"
            value="investment"
            v-model="type"
            class="input yellow"
          />
          <span class="mt-2 text-sm font-medium">Investimento</span>
        </label>
        <label class="flex flex-col items-center cursor-pointer">
          <input
            type="radio"
            name="tipo"
            value="expense"
            v-model="type"
            class="input red"
          />
          <span class="mt-2 text-sm font-medium">Despesa</span>
        </label>
      </div>

      <!-- Data -->
      <div class="mb-4">
        <label for="date" class="block mb-1 font-medium text-gray-700">Data</label>
        <input
          id="date"
          v-model="date"
          type="date"
          class="w-full border rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-400"
          :class="{ 'border-red-500': errors.date }"
        />
        <p v-if="errors.date" class="mt-1 text-sm text-red-600">{{ errors.date }}</p>
      </div>

      <!-- Descrição -->
      <div class="mb-4">
        <label for="description" class="block mb-1 font-medium text-gray-700">Descrição</label>
        <template v-if="type === 'investment'">
          <select
              id="description"
              v-model="description"
              class="w-full border rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-400"
              :class="{ 'border-red-500': errors.description }"
          >
            <option value="">Selecione um investimento</option>
            <option v-for="ativo in investmentsStore.ativosLite" :key="ativo.id" :value="ativo.nome">
              {{ ativo.nome }}
            </option>
          </select>
        </template>
        <template v-else>
          <input
              id="description"
              v-model="description"
              type="text"
              class="w-full border rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-400"
              :class="{ 'border-red-500': errors.description }"
              placeholder="ex: Salário, Aluguel, Supermercado"
          />
        </template>
        <input
          id="description"
          v-model="description"
          type="text"
          class="w-full border rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-400"
          :class="{ 'border-red-500': errors.description }"
          placeholder="ex: Salário, Aluguel, Supermercado"
        />
        <p v-if="errors.description" class="mt-1 text-sm text-red-600">{{ errors.description }}</p>
      </div>

      <!-- Categoria -->
      <div class="mb-4">
        <label for="category" class="block mb-1 font-medium text-gray-700">Categoria</label>
        <select
          id="category"
          v-model="category"
          class="w-full border rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-400"
        >
          <option
            v-for="cat in transactionsStore.categories"
            :key="cat"
            :value="cat"
          >
            {{ cat }}
          </option>
        </select>
      </div>

      <!-- Valor -->
      <div class="mb-6">
        <label for="amount" class="block mb-1 font-medium text-gray-700">Valor</label>
        <input
          id="amount"
          v-model.number="amount"
          type="number"
          step="0.01"
          class="w-full border rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-400"
          :class="{ 'border-red-500': errors.amount }"
          placeholder="0,00"
        />
        <p v-if="errors.amount" class="mt-1 text-sm text-red-600">{{ errors.amount }}</p>
      </div>

      <!-- Botões -->
      <div class="flex justify-end space-x-3">
        <button
          type="button"
          @click="emit('close')"
          class="btn btn-secondary"
        >
          Cancelar
        </button>
        <button
          type="submit"
          class="btn btn-primary"
        >
          {{ props.isEditing ? 'Atualizar' : 'Adicionar' }} Transação
        </button>
      </div>
    </form>
  </div>
</template>

<style scoped>
.radio-input {
  display: grid;
  grid-template-columns: repeat(3, auto);
  gap: 1.5rem;
  justify-items: center;
  margin-bottom: 1.5rem;
}

/* CSS dos “dots” */
.input {
  -webkit-appearance: none;
  margin: 6px;
  width: 24px;
  height: 24px;
  border-radius: 12px;
  cursor: pointer;
  vertical-align: middle;
  box-shadow:
    hsl(0, 0%, 100%) 0 1px 1px,
    inset hsla(0, 0%, 100%, 0.5) 0 0 0 1px;
  background-color: hsla(0, 0%, 100%, 0.2);
  background-repeat: no-repeat;
  transition:
    background-position 0.15s cubic-bezier(0.8,0,1,1),
    transform            0.25s cubic-bezier(0.8,0,1,1);
  outline: none;
}
.input.green {
  background-image: radial-gradient(
    hsla(118,100%,90%,1) 0%,
    hsla(118,100%,70%,1) 15%,
    hsla(118,100%,60%,0.3) 28%,
    hsla(118,100%,30%,0) 70%
  );
}
.input.yellow {
  background-image: radial-gradient(
    hsla(50,100%,90%,1) 0%,
    hsla(50,100%,70%,1) 15%,
    hsla(50,100%,60%,0.3) 28%,
    hsla(50,100%,30%,0) 70%
  );
}
.input.red {
  background-image: radial-gradient(
    hsla(0,100%,90%,1) 0%,
    hsla(0,100%,70%,1) 15%,
    hsla(0,100%,60%,0.3) 28%,
    hsla(0,100%,30%,0) 70%
  );
}
.input,
.input:active {
  background-position: 24px 0;
}
.input:checked {
  background-position: 0 0;
  transition:
    background-position 0.2s 0.15s cubic-bezier(0,0,0.2,1),
    -webkit-transform 0.25s cubic-bezier(0,0,0.2,1);
}
.input:active {
  transform: scale(1.5);
  transition: transform 0.1s cubic-bezier(0,0,0.2,1);
}
/* desloca os irmãos do right */
.input:checked ~ .input,
.input:checked ~ .input:active {
  background-position: -24px 0;
}
</style>
