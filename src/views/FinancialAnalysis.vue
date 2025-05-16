<template>
  <div class="financial-analysis-view">
    <div class="header">
      <h1><i class="fas fa-chart-line"></i> Análise Financeira Inteligente</h1>
      <p class="subtitle">Obtenha insights personalizados sobre seus hábitos financeiros</p>
    </div>

    <div class="content-card">
      <div class="analysis-controls">
        <button 
          @click="analyzeFinances" 
          :disabled="isLoading || !hasTransactions"
          class="analyze-button"
        >
          <span v-if="!isLoading">
            <i class="fas fa-magic"></i> Gerar Análise
          </span>
          <span v-else>
            <i class="fas fa-spinner fa-spin"></i> Analisando...
          </span>
        </button>
        
        <div v-if="!hasTransactions" class="warning-message">
          <i class="fas fa-exclamation-triangle"></i> Adicione transações para habilitar a análise
        </div>
      </div>

      <div v-if="error" class="error-message">
        <i class="fas fa-exclamation-circle"></i> {{ error }}
      </div>

      <div v-if="advice" class="advice-container">
        <div class="advice-header">
          <h2><i class="fas fa-lightbulb"></i> Recomendações Personalizadas</h2>
          <button @click="copyAdvice" class="copy-button">
            <i class="far fa-copy"></i> Copiar
          </button>
        </div>
        
        <div class="advice-content" v-html="formattedAdvice"></div>
        
        <div class="feedback-buttons">
          <button @click="sendFeedback('helpful')" class="feedback-button helpful">
            <i class="fas fa-thumbs-up"></i> Útil
          </button>
          <button @click="sendFeedback('not-helpful')" class="feedback-button not-helpful">
            <i class="fas fa-thumbs-down"></i> Não útil
          </button>
        </div>
      </div>

      <div v-else-if="!isLoading" class="empty-state">
        <img src="" alt="Análise financeira" class="empty-image">
        <h3>Descubra insights sobre suas finanças</h3>
        <p>Clique no botão acima para receber uma análise completa do seu histórico financeiro, com:</p>
        <ul>
          <li>Identificação de padrões de gastos</li>
          <li>Sugestões de economia personalizadas</li>
          <li>Recomendações de orçamento</li>
          <li>Metas financeiras inteligentes</li>
        </ul>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '../stores/auth';
import { useTransactionsStore } from '../stores/transactions';
import { geminiService } from '../services/iaService';

const authStore = useAuthStore();
const transactionsStore = useTransactionsStore();
const router = useRouter();

const isLoading = ref(false);
const error = ref('');
const advice = ref('');

// Verificação melhorada de transações
const hasTransactions = computed(() => {
  return transactionsStore.transactions.length > 0 && 
         !transactionsStore.transactions.some(t => isNaN(t.amount));
});

// Carregamento seguro de dados
const loadData = async () => {
  try {
    if (!authStore.isAuthenticated) {
      await router.push({ name: 'login' });
      return false;
    }

    await transactionsStore.loadTransactions();
    
    // Verificação adicional dos dados
    if (transactionsStore.transactions.some(t => isNaN(t.amount))) {
      throw new Error('Dados de transações inválidos');
    }
    
    return true;
  } catch (err) {
    console.error('Erro ao carregar transações:', err);
    error.value = 'Erro ao carregar dados financeiros';
    return false;
  }
};

// Análise financeira com tratamento completo
const analyzeFinances = async () => {
  if (!authStore.isAuthenticated) {
    await router.push({ name: 'login' });
    return;
  }

  isLoading.value = true;
  error.value = '';
  advice.value = '';

  try {
    const loaded = await loadData();
    
    if (!loaded || !hasTransactions.value) {
      error.value = 'Dados insuficientes para análise. Adicione transações válidas.';
      return;
    }

    // Timeout para evitar espera infinita
    const timeout = new Promise((_, reject) => 
      setTimeout(() => reject(new Error('Tempo excedido na análise')), 30000)
    );

    advice.value = await Promise.race([
      geminiService.getFinancialAdvice(transactionsStore.transactions),
      timeout
    ]);
    
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Erro desconhecido';
    console.error('Erro na análise:', err);
  } finally {
    isLoading.value = false;
  }
};

// Formatação segura do conselho
const formattedAdvice = computed(() => {
  if (!advice.value) return '';
  return advice.value
    .replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>')
    .replace(/\n/g, '<br>');
});

onMounted(async () => {
  if (!authStore.isAuthenticated) {
    await router.push({ name: 'login' });
  }
});
</script>

<style scoped>
.financial-analysis-view {
  max-width: 900px;
  margin: 0 auto;
  padding: 1.5rem;
}

.header {
  text-align: center;
  margin-bottom: 2rem;
}

.header h1 {
  color: #2c3e50;
  margin-bottom: 0.5rem;
}

.subtitle {
  color: #7f8c8d;
  font-size: 1.1rem;
}

.content-card {
  background-color: white;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  padding: 2rem;
}

.analysis-controls {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-bottom: 1.5rem;
}

.analyze-button {
  background-color: #4CAF50;
  color: white;
  border: none;
  padding: 12px 24px;
  font-size: 1rem;
  border-radius: 30px;
  cursor: pointer;
  transition: all 0.3s;
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 600;
}

.analyze-button:hover:not(:disabled) {
  background-color: #3d8b40;
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.analyze-button:disabled {
  background-color: #b0bec5;
  cursor: not-allowed;
}

.warning-message {
  margin-top: 1rem;
  color: #ff9800;
  display: flex;
  align-items: center;
  gap: 8px;
}

.error-message {
  color: #e74c3c;
  padding: 1rem;
  background-color: #fdecea;
  border-radius: 8px;
  margin-bottom: 1.5rem;
  display: flex;
  align-items: center;
  gap: 8px;
}

.advice-container {
  margin-top: 2rem;
  border-top: 1px solid #eee;
  padding-top: 1.5rem;
}

.advice-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.copy-button {
  background-color: #f5f5f5;
  border: 1px solid #ddd;
  padding: 6px 12px;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  gap: 6px;
}

.copy-button:hover {
  background-color: #e0e0e0;
}

.advice-content {
  line-height: 1.8;
  padding: 1rem;
  background-color: #f9f9f9;
  border-radius: 8px;
  border-left: 4px solid #4CAF50;
}

.empty-state {
  text-align: center;
  padding: 2rem 0;
}

.empty-image {
  max-width: 200px;
  opacity: 0.7;
  margin-bottom: 1.5rem;
}

.empty-state h3 {
  color: #2c3e50;
  margin-bottom: 1rem;
}

.empty-state ul {
  text-align: left;
  display: inline-block;
  margin-top: 1rem;
}

.feedback-buttons {
  display: flex;
  justify-content: center;
  gap: 1rem;
  margin-top: 1.5rem;
}

.feedback-button {
  padding: 8px 16px;
  border-radius: 20px;
  border: none;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 6px;
  transition: all 0.2s;
}

.feedback-button.helpful {
  background-color: #e8f5e9;
  color: #2e7d32;
}

.feedback-button.not-helpful {
  background-color: #ffebee;
  color: #c62828;
}

.feedback-button:hover {
  transform: translateY(-2px);
}

/* Rich text formatting */
.advice-content :deep(strong) {
  color: #2c3e50;
}

.advice-content :deep(h3) {
  color: #4CAF50;
  margin-top: 1.5rem;
  margin-bottom: 0.5rem;
}

.advice-content :deep(br) {
  content: "";
  display: block;
  margin-bottom: 0.8rem;
}

.advice-content :deep(ul) {
  padding-left: 1.5rem;
  margin: 1rem 0;
}

.advice-content :deep(li) {
  margin-bottom: 0.5rem;
  position: relative;
  padding-left: 1.5rem;
}

.advice-content :deep(li):before {
  content: "•";
  color: #4CAF50;
  font-weight: bold;
  position: absolute;
  left: 0;
}
</style>