<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useInvestmentsStore, Investment } from '../stores/investiments.ts'
import InvestmentForm from '../components/ui/InvestmentForm.vue'
import InvestmentSummary from '../components/investments/InvestmentSummary.vue'
import InvestmentChart from '../components/investments/InvestmentChart.vue'

const router = useRouter()
const authStore = useAuthStore()
const investmentsStore = useInvestmentsStore()

const showForm = ref(false)
const isEditing = ref(false)
const currentInvestment = ref<Investment | undefined>(undefined)
const searchQuery = ref('')
const selectedType = ref('')

const openAddForm = () => {
  isEditing.value = false
  currentInvestment.value = undefined
  showForm.value = true
}

const openEditForm = (investment: Investment) => {
  isEditing.value = true
  currentInvestment.value = investment
  showForm.value = true
}

const closeForm = () => {
  showForm.value = false
}

const deleteInvestment = async (id: string) => {
  if (confirm('Tem certeza que deseja excluir este investimento?')) {
    await investmentsStore.deleteInvestment(id)
  }
}

const filteredInvestments = computed(() => {
  return investmentsStore.investments.filter(investment => {
    const matchesSearch = searchQuery.value === '' ||
        investment.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
        investment.broker.toLowerCase().includes(searchQuery.value.toLowerCase())

    const matchesType = selectedType.value === '' ||
        investment.type === selectedType.value

    return matchesSearch && matchesType
  }).sort((a, b) => new Date(b.purchaseDate).getTime() - new Date(a.purchaseDate).getTime())
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

const getTypeLabel = (type: string) => {
  const typeObj = investmentsStore.investmentTypes.find(t => t.value === type)
  return typeObj ? typeObj.label : type
}

const getReturnPercentage = (investment: Investment) => {
  const returnValue = investment.currentValue - investment.amount
  const percentage = (returnValue / investment.amount) * 100
  return percentage
}

const getReturnClass = (investment: Investment) => {
  const returnValue = investment.currentValue - investment.amount
  return returnValue >= 0 ? 'text-success' : 'text-danger'
}

const resetFilters = () => {
  searchQuery.value = ''
  selectedType.value = ''
}

onMounted(async () => {
  if (!authStore.isAuthenticated) {
    router.push('/login')
    return
  }
  await investmentsStore.loadInvestments()
})
</script>

<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <div class="flex flex-col md:flex-row md:items-center md:justify-between mb-6">
      <h1 class="text-2xl font-bold text-gray-900 mb-4 md:mb-0">Investimentos</h1>
      <button @click="openAddForm" class="btn btn-primary">
        Adicionar Investimento
      </button>
    </div>

    <!-- Summary Cards -->
    <div class="mb-6">
      <InvestmentSummary />
    </div>

    <!-- Chart -->
    <div class="mb-6">
      <InvestmentChart />
    </div>

    <!-- Filters -->
    <div class="bg-white p-4 rounded-lg shadow-md mb-6">
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div>
          <label for="search" class="label">Pesquisar</label>
          <input
              id="search"
              v-model="searchQuery"
              type="text"
              class="input"
              placeholder="Pesquisar investimentos..."
          />
        </div>
        <div>
          <label for="type" class="label">Tipo de Investimento</label>
          <select id="type" v-model="selectedType" class="input">
            <option value="">Todos os Tipos</option>
            <option v-for="type in investmentsStore.investmentTypes" :key="type.value" :value="type.value">
              {{ type.label }}
            </option>
          </select>
        </div>
        <div class="flex items-end">
          <button @click="resetFilters" class="btn btn-secondary w-full">
            Limpar Filtros
          </button>
        </div>
      </div>
    </div>

    <!-- Investments Table -->
    <div class="bg-white rounded-lg shadow-md overflow-hidden">
      <div v-if="investmentsStore.isLoading" class="px-6 py-4 text-sm text-gray-500">
        Carregando investimentos...
      </div>
      <div v-else-if="investmentsStore.error" class="px-6 py-4 text-sm text-red-600">
        {{ investmentsStore.error }}
      </div>
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200" v-if="!investmentsStore.isLoading && !investmentsStore.error">
          <thead class="bg-gray-50">
          <tr>
            <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Nome
            </th>
            <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Tipo
            </th>
            <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Corretora
            </th>
            <th scope="col" class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              Valor Investido
            </th>
            <th scope="col" class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              Valor Atual
            </th>
            <th scope="col" class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              Retorno
            </th>
            <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Data de Compra
            </th>
            <th scope="col" class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              Ações
            </th>
          </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="investment in filteredInvestments" :key="investment.id">
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
              {{ investment.name }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ getTypeLabel(investment.type) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ investment.broker }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-right font-medium text-gray-900">
              {{ formatCurrency(investment.amount) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-right font-medium text-gray-900">
              {{ formatCurrency(investment.currentValue) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-right font-medium" :class="getReturnClass(investment)">
              {{ formatCurrency(investment.currentValue - investment.amount) }}
              <br>
              <span class="text-xs">
                  ({{ getReturnPercentage(investment).toFixed(2) }}%)
                </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ formatDate(investment.purchaseDate) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
              <button @click="openEditForm(investment)" class="text-primary-600 hover:text-primary-900 mr-3">
                Editar
              </button>
              <button @click="deleteInvestment(investment.id)" class="text-danger hover:text-red-700">
                Excluir
              </button>
            </td>
          </tr>
          <tr v-if="!investmentsStore.isLoading && filteredInvestments.length === 0">
            <td colspan="8" class="px-6 py-4 text-center text-sm text-gray-500">
              Nenhum investimento encontrado
            </td>
          </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Investment Form Modal -->
    <div v-if="showForm" class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center z-50 p-4">
      <div class="max-w-2xl w-full max-h-screen overflow-y-auto">
        <InvestmentForm
            :investment="currentInvestment"
            :is-editing="isEditing"
            @close="closeForm"
        />
      </div>
    </div>
  </div>
</template>