<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useTransactionsStore, Transaction } from '../stores/transactions'
import TransactionForm from '../components/ui/TransactionForm.vue'
import ImportCsv from '../components/ImportCsv.vue'

const router = useRouter()
const authStore = useAuthStore()
const transactionsStore = useTransactionsStore()

const showCsvImporter = ref(false)
function openCsvImporter() {
  showCsvImporter.value = true
}

const showForm = ref(false)
const isEditing = ref(false)
const currentTransaction = ref<Transaction | undefined>(undefined)

const searchQuery = ref('')
const selectedCategory = ref('')
const selectedType = ref('')
const dateFrom = ref('')
const dateTo = ref('')

function openAddForm() {
  isEditing.value = false
  currentTransaction.value = undefined
  showForm.value = true
}

function openEditForm(tx: Transaction) {
  isEditing.value = true
  currentTransaction.value = tx
  showForm.value = true
}

function closeForm() {
  showForm.value = false
}

function deleteTransaction(id: string) {
  if (confirm('Tem certeza que deseja excluir esta transação?')) {
    transactionsStore.deleteTransaction(id)
  }
}

const filteredTransactions = computed(() =>
  transactionsStore.transactions
    .filter(tx => {
      const matchesSearch =
        !searchQuery.value ||
        tx.description.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
        tx.category.toLowerCase().includes(searchQuery.value.toLowerCase())

      const matchesCategory =
        !selectedCategory.value || tx.category === selectedCategory.value

      const matchesType =
        !selectedType.value || tx.type === selectedType.value

      let matchesDateRange = true
      if (dateFrom.value) matchesDateRange &&= tx.date >= dateFrom.value
      if (dateTo.value) matchesDateRange &&= tx.date <= dateTo.value

      return matchesSearch && matchesCategory && matchesType && matchesDateRange
    })
    .sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
)

function formatCurrency(v: number) {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL'
  }).format(v)
}

function formatDate(d: string) {
  return new Date(d).toLocaleDateString('pt-BR', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

function resetFilters() {
  searchQuery.value = ''
  selectedCategory.value = ''
  selectedType.value = ''
  dateFrom.value = ''
  dateTo.value = ''
}

onMounted(async () => {
  if (!authStore.isAuthenticated) {
    router.push('/login')
  } else {
    await transactionsStore.loadTransactions()
  }
})
</script>

<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <div class="flex flex-col md:flex-row md:items-center md:justify-between mb-6">
      <h1 class="text-2xl font-bold text-gray-900 mb-4 md:mb-0">Transações</h1>
      <div class="flex  space-x-2">
        <button @click="openAddForm" class="btn btn-primary">
          Adicionar Transação
        </button>
        <button @click="openCsvImporter" class="btn btn-secondary">
          Importar planilha
        </button>
      </div>
    </div>

    <ImportCsv
      v-if="showCsvImporter"
      @close="showCsvImporter = false"
    />

    <div class="bg-white p-4 rounded-lg shadow-md mb-6">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div>
          <label for="search" class="label">Pesquisar</label>
          <input id="search" v-model="searchQuery" type="text" class="input" placeholder="Pesquisar transações..." />
        </div>
        <div>
          <label for="category" class="label">Categoria</label>
          <select id="category" v-model="selectedCategory" class="input">
            <option value="">Todas as Categorias</option>
            <option v-for="category in transactionsStore.categories" :key="category" :value="category">
              {{ category }}
            </option>
          </select>
        </div>
        <div>
          <label for="type" class="label">Tipo</label>
          <select id="type" v-model="selectedType" class="input">
            <option value="">Todos os Tipos</option>
            <option value="income">Receita</option>
            <option value="expense">Despesa</option>
          </select>
        </div>
        <div class="grid grid-cols-2 gap-2">
          <div>
            <label for="dateFrom" class="label">De</label>
            <input id="dateFrom" v-model="dateFrom" type="date" class="input" />
          </div>
          <div>
            <label for="dateTo" class="label">Até</label>
            <input id="dateTo" v-model="dateTo" type="date" class="input" />
          </div>
        </div>
      </div>
      <div>

      </div>
      <div class="mt-4 flex justify-end">

      </div>
      <div class="mt-4 flex justify-end">
        <button @click="resetFilters" class="btn btn-secondary">
          Limpar Filtros
        </button>
      </div>
    </div>

    <!-- Transactions Table -->
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
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Tipo
              </th>
              <th scope="col" class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                Valor
              </th>
              <th scope="col" class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                Ações
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
              <td class="px-6 py-4 whitespace-nowrap text-sm">
                <span class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full"
                  :class="transaction.type === 'income' ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'">
                  {{ transaction.type === 'income' ? 'Receita' : 'Despesa' }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-right font-medium"
                :class="transaction.type === 'income' ? 'text-success' : 'text-danger'">
                {{ formatCurrency(transaction.amount) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <button @click="openEditForm(transaction)" class="text-primary-600 hover:text-primary-900 mr-3">
                  Editar
                </button>
                <button @click="deleteTransaction(transaction.id)" class="text-danger hover:text-red-700">
                  Excluir
                </button>
              </td>
            </tr>
            <tr v-if="filteredTransactions.length === 0">
              <td colspan="6" class="px-6 py-4 text-center text-sm text-gray-500">
                Nenhuma transação encontrada
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div v-if="showForm" class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center z-50 p-4">
      <div class="max-w-md w-full">
        <TransactionForm :transaction="currentTransaction" :is-editing="isEditing" @close="closeForm" />
      </div>
    </div>
  </div>
</template>