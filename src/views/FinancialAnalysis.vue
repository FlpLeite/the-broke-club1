<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
    <!-- Cabeçalho com botão azul -->
    <div class="flex flex-col md:flex-row md:items-center md:justify-between mb-6">
      <h1 class="text-2xl font-bold text-gray-900 mb-4 md:mb-0">
        Análise Financeira Inteligente
      </h1>
      <button
        @click="analyzeFinances"
        :disabled="isLoading || !hasTransactions"
        class="btn btn-primary"
      >
        <span v-if="isLoading">
          <i class="fas fa-spinner fa-spin mr-2"></i> Analisando...
        </span>
        <span v-else>
          <i class="fas fa-magic mr-2"></i> Gerar Análise
        </span>
      </button>
    </div>

    <!-- Card principal -->
    <div class="bg-white rounded-lg shadow-md overflow-hidden">
      <div class="p-6">

        <!-- Erro -->
        <div v-if="error" class="bg-red-50 text-red-700 p-4 rounded mb-6 flex items-center gap-2">
          <i class="fas fa-exclamation-circle"></i>
          {{ error }}
        </div>

        <!-- Conteúdo da análise -->
        <div v-if="advice">
          <!-- Cabeçalho da recomendação -->
          <div class="flex justify-between items-center mb-4">
            <h2 class="text-lg font-semibold flex items-center gap-2">
              <i class="fas fa-lightbulb text-blue-600"></i>
              Recomendações Personalizadas
            </h2>
            <button @click="copyAdvice" class="btn btn-secondary btn-sm flex items-center gap-1">
              <i class="far fa-copy"></i> Copiar
            </button>
          </div>

          <!-- Aqui usamos `renderedAdvice` com marcado HTML do Markdown -->
          <div
            class="prose prose-sm sm:prose lg:prose-lg max-w-none bg-gray-50 border-l-4 border-blue-600 p-4 rounded overflow-y-auto max-h-[32rem]"
            v-html="renderedAdvice"
          ></div>       
        </div>

        <!-- Estado inicial -->
        <div v-else-if="!isLoading" class="text-center text-gray-600">
          <h3 class="text-lg font-medium mb-2">Descubra insights sobre suas finanças</h3>
          <p>Clique no botão para receber uma análise completa do seu histórico financeiro.</p>
        </div>

        <!-- Spinner no card -->
        <div v-else class="text-center py-12">
          <i class="fas fa-spinner fa-spin text-2xl text-gray-400"></i>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { marked } from 'marked'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useTransactionsStore } from '../stores/transactions'
import { geminiService } from '../services/iaService'

const authStore         = useAuthStore()
const transactionsStore = useTransactionsStore()
const router            = useRouter()

const isLoading = ref(false)
const error     = ref('')
const advice    = ref('')

const hasTransactions = computed(() =>
  transactionsStore.transactions.length > 0 &&
  !transactionsStore.transactions.some(t => isNaN(t.amount))
)

async function analyzeFinances() {
  if (!authStore.isAuthenticated) {
    await router.push({ name: 'login' })
    return
  }

  isLoading.value = true
  error.value     = ''
  advice.value    = ''

  try {
    await transactionsStore.loadTransactions()
    if (!hasTransactions.value) {
      throw new Error('Adicione transações válidas para análise.')
    }

    const timeout = new Promise((_, rej) =>
      setTimeout(() => rej(new Error('Tempo excedido na análise')), 30000)
    )

    advice.value = (await Promise.race([
      geminiService.getFinancialAdvice(transactionsStore.transactions),
      timeout
    ])) as string
  } catch (err: any) {
    error.value = err.message || 'Erro na análise'
  } finally {
    isLoading.value = false
  }
}

function copyAdvice() {
  navigator.clipboard.writeText(advice.value)
}

function sendFeedback(type: 'helpful' | 'not-helpful') {
  console.log('Feedback:', type)
}

const renderedAdvice = computed(() => {
  return marked(advice.value || '')
})
</script>

<style scoped>
.prose h1 { @apply text-3xl font-bold text-gray-900 mb-4; }
.prose h2 { @apply text-2xl font-semibold text-gray-800 mt-8 mb-4; }
.prose h3 { @apply text-xl font-semibold text-gray-800 mt-6 mb-3; }
.prose ul { @apply list-disc list-inside mb-4; }
.prose li { @apply mb-2; }
.prose p  { @apply mb-4; }
.prose strong { @apply font-semibold text-gray-900; }
.prose {
  -ms-overflow-style: none;
  scrollbar-width: none;
}
</style>
