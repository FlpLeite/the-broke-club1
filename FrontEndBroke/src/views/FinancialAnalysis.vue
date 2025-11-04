<template>
  <div class="financial-analysis-view">
    <div class="header">
      <h1><i class="fas fa-chart-line"></i> Assistente Financeira Penny</h1>
      <p class="subtitle">Escolha o nível de análise para insights personalizados</p>
    </div>

    <div class="tabs">
      <button
        @click="activeTab = 'analysis'"
        :class="{ active: activeTab === 'analysis' }"
        class="tab-button"
      >
        <i class="fas fa-chart-pie"></i> Análise Automática
      </button>
      <button
        @click="activeTab = 'chat'"
        :class="{ active: activeTab === 'chat' }"
        class="tab-button"
      >
        <i class="fas fa-comments"></i> Chat com a Penny
      </button>
    </div>

    <AnalysisTab 
      v-if="activeTab === 'analysis'"
      :has-transactions="hasTransactions"
    />

    <ChatTab
      v-if="activeTab === 'chat'"
      :has-transactions="hasTransactions"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '../stores/auth';
import { useTransactionsStore } from '../stores/transactions';
import AnalysisTab from '../components/FinancialAnalysis/IaAnalysisFixed.vue';
import ChatTab from '../components/FinancialAnalysis/IaChat.vue';

const authStore = useAuthStore();
const transactionsStore = useTransactionsStore();
const router = useRouter();

const activeTab = ref<'analysis' | 'chat'>('analysis');

const hasTransactions = computed(() => {
  return transactionsStore.transactions.length > 0 &&
    !transactionsStore.transactions.some(t => isNaN(t.amount));
});

const loadData = async () => {
  try {
    if (!authStore.isAuthenticated) {
      await router.push({ name: 'login' });
      return false;
    }

    await transactionsStore.loadTransactions();

    if (transactionsStore.transactions.some(t => isNaN(t.amount))) {
      throw new Error('Dados de transações inválidos');
    }

    return true;
  } catch (err) {
    console.error('Erro ao carregar transações:', err);
    return false;
  }
};

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

.tabs {
  display: flex;
  justify-content: center;
  margin-bottom: 1.5rem;
  border-bottom: 1px solid #eee;
}

.tab-button {
  background: none;
  border: none;
  padding: 0.75rem 1.5rem;
  font-size: 1rem;
  cursor: pointer;
  color: #7f8c8d;
  border-bottom: 3px solid transparent;
  transition: all 0.3s;
  display: flex;
  align-items: center;
  gap: 8px;
}

.tab-button:hover {
  color: #2c3e50;
}

.tab-button.active {
  color: #0062a3;
  border-bottom-color: #4c90af;
  font-weight: 600;
}

.dark .tab-button.active {
  color: #40cbf5;
  border-bottom-color: #45b4ff;
}
</style>