<template>
  <div class="financial-analysis-view">
    <div class="header">
      <h1><i class="fas fa-chart-line"></i> Assistente Financeira Penny</h1>
      <p class="subtitle">Obtenha insights personalizados sobre seus hábitos financeiros</p>
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

    <div v-if="activeTab === 'analysis'" class="content-card">
      <!-- Conteúdo existente da análise -->
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
        <div class="advice-content" v-html="formattedAdvice"></div>
      </div>

<div v-else-if="!isLoading" class="empty-state">
  <div class="empty-image-container">
    <img src="../assets/Penny.png" alt="Penny - Assistente Financeira" class="empty-image">
    </div>
    <strong><h1 class="Apresentacao">Ola me chamo Penny sou IA Assistente da The Broke club</h1></strong>
    <p>estou aqui para auxilia-lo com suas duvidas e ajudar a organizar sua situação financeira</p>
    </div>
    </div>

    <div v-if="activeTab === 'chat'" class="content-card chat-container">
      <div class="chat-messages" ref="chatContainer">
        <div
          v-for="(message, index) in chatMessages"
          :key="index"
          :class="['message', message.role]"
          >
          <div class="message-header">
            <img
                :src="message.role === 'user' ? userAvatar : pennyAvatar"
                :alt="message.role === 'user' ? 'Seu avatar' : 'Penny - Assistente Financeira'"
                class="empty-image-profile"
            />
            <strong>{{ message.role === 'user' ? 'Você' : 'Penny' }}</strong>
            <span class="message-time">{{ formatTime(message.timestamp) }}</span>
          </div>
          <div class="message-content" v-html="formatMessage(message.content)"></div>
        </div>

        <div v-if="isChatLoading" class="message assistant">
          <div class="message-header">
            <strong>Penny</strong>
          </div>
          <div class="message-content">
            <i class="fas fa-spinner fa-spin"></i> Pensando...
          </div>
        </div>
      </div>

      <div class="chat-input-container">
        <form @submit.prevent="sendMessage">
          <div class="input-group">
            <input
              v-model="userMessage"
              type="text"
              placeholder="Pergunte algo sobre suas finanças..."
              :disabled="isChatLoading || !hasTransactions"
              class="chat-input"
            />
            <button
              type="submit"
              :disabled="!userMessage || isChatLoading || !hasTransactions"
              class="send-button"
            >
              <i class="fas fa-paper-plane"></i>
            </button>
          </div>

          <div v-if="!hasTransactions" class="warning-message">
            <i class="fas fa-exclamation-triangle"></i> Adicione transações para habilitar o chat
          </div>
        </form>

        <div class="suggestions">
          <p>Tente perguntar:</p>
          <button
            v-for="(suggestion, index) in suggestedQuestions"
            :key="index"
            @click="userMessage = suggestion; sendMessage()"
            class="suggestion-button"
          >
            {{ suggestion }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, nextTick } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '../stores/auth';
import { useTransactionsStore } from '../stores/transactions';
import { geminiService } from '../services/iaService';
import pennyAvatar from '../assets/Penny.png'
import userAvatar from '../assets/user.svg'


const authStore = useAuthStore();
const transactionsStore = useTransactionsStore();
const router = useRouter();

// Variáveis para a aba de análise
const isLoading = ref(false);
const error = ref('');
const advice = ref('');
const activeTab = ref('analysis');

// Variáveis para o chat
const chatMessages = ref<Array<{
  role: 'user' | 'assistant';
  content: string;
  timestamp: Date;
}>>([]);
const userMessage = ref('');
const isChatLoading = ref(false);
const chatContainer = ref<HTMLElement | null>(null);

const suggestedQuestions = [
  "Quais são meus maiores gastos este mês?",
  "Como posso economizar mais dinheiro?",
  "Estou gastando muito com alimentação?",
  "Quais categorias posso reduzir meus gastos?",
  "Como está meu saldo atual?"
];

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
    error.value = 'Erro ao carregar dados financeiros';
    return false;
  }
};

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

const sendMessage = async () => {
  if (!userMessage.value.trim() || isChatLoading.value || !hasTransactions.value) return;

  // Adiciona mensagem do usuário ao chat
  const userMsg = {
    role: 'user' as const,
    content: userMessage.value,
    timestamp: new Date()
  };
  chatMessages.value.push(userMsg);

  const message = userMessage.value;
  userMessage.value = '';
  isChatLoading.value = true;

  try {
    // Chama o serviço da IA
    const response = await geminiService.chatWithAI({
      message,
      transactions: transactionsStore.transactions,
      chatHistory: chatMessages.value.slice(0, -1) // Exclui a última mensagem (a atual)
    });

    // Adiciona resposta da IA
    chatMessages.value.push({
      role: 'assistant' as const,
      content: response,
      timestamp: new Date()
    });

  } catch (err) {
    console.error('Erro no chat:', err);
    chatMessages.value.push({
      role: 'assistant' as const,
      content: 'Desculpe, ocorreu um erro ao processar sua mensagem. Por favor, tente novamente.',
      timestamp: new Date()
    });
  } finally {
    isChatLoading.value = false;
    scrollToBottom();
  }
};

const formatMessage = (content: string) => {
  return content
    .replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>')
    .replace(/\n/g, '<br>')
    .replace(/- (.*?)(<br>|$)/g, '<li>$1</li>');
};

const formatTime = (date: Date) => {
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
};

const scrollToBottom = () => {
  nextTick(() => {
    if (chatContainer.value) {
      chatContainer.value.scrollTop = chatContainer.value.scrollHeight;
    }
  });
};

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

  // Mensagem inicial do chat
  chatMessages.value.push({
    role: 'assistant',
    content: 'Olá! Eu sou a Penny, sua assistente financeira. Como posso te ajudar hoje?',
    timestamp: new Date()
  });
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


.Apresentacao{
  font-size: 140%;
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
  color: #4CAF50;
  border-bottom-color: #4CAF50;
  font-weight: 600;
}

.content-card {
  background-color: white;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  padding: 2rem;
}

.chat-container {
  display: flex;
  flex-direction: column;
  height: 600px;
}

.chat-messages {
  flex: 1;
  overflow-y: auto;
  padding: 1rem;
  margin-bottom: 1rem;
  border-radius: 8px;
  background-color: #f9f9f9;
}

.message {
  margin-bottom: 1rem;
  padding: 0.75rem 1rem;
  border-radius: 8px;
  max-width: 80%;
}

.message.user {
  margin-left: auto;
  background-color: #e3f2fd;
  border-bottom-right-radius: 0;
}

.message.assistant {
  margin-right: auto;
  background-color: #f1f1f1;
  border-bottom-left-radius: 0;
}

.message-header {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 0.5rem;
  font-size: 0.85rem;
  color: #555;
}

.message-time {
  font-size: 0.75rem;
  opacity: 0.7;
}

.message-content {
  line-height: 1.5;
}

.chat-input-container {
  margin-top: auto;
}

.input-group {
  display: flex;
  gap: 8px;
}

.chat-input {
  flex: 1;
  padding: 12px 16px;
  border: 1px solid #ddd;
  border-radius: 24px;
  font-size: 1rem;
  transition: all 0.3s;
}

.chat-input:focus {
  outline: none;
  border-color: #4CAF50;
  box-shadow: 0 0 0 2px rgba(76, 175, 80, 0.2);
}

.send-button {
  width: 48px;
  height: 48px;
  border-radius: 50%;
  border: none;
  background-color: #4CAF50;
  color: white;
  cursor: pointer;
  transition: all 0.3s;
  display: flex;
  align-items: center;
  justify-content: center;
}

.send-button:hover:not(:disabled) {
  background-color: #3d8b40;
  transform: scale(1.05);
}

.send-button:disabled {
  background-color: #b0bec5;
  cursor: not-allowed;
}

.suggestions {
  margin-top: 1rem;
}

.suggestions p {
  font-size: 0.9rem;
  color: #7f8c8d;
  margin-bottom: 0.5rem;
}

.suggestion-button {
  background-color: #f5f5f5;
  border: 1px solid #ddd;
  border-radius: 16px;
  padding: 6px 12px;
  margin-right: 8px;
  margin-bottom: 8px;
  font-size: 0.85rem;
  cursor: pointer;
  transition: all 0.2s;
}

.suggestion-button:hover {
  background-color: #e0e0e0;
}

/* Estilos existentes mantidos */
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

.empty-image-container {
  display: flex;
  justify-content: center;
  margin-bottom: 1.5rem;
}

.empty-image {
  width: 150px;
  height: 150px;
  border-radius: 50%;
  object-fit: cover;
  border: 4px solid #4CAF50;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.empty-image-profile{
  width: 50px;
  height: 50px;
  border-radius: 50%;
  object-fit: cover;
  border: 4px solid #4CAF50;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.empty-state h3 {
  color: #2c3e50;
  margin-bottom: 1rem;
}

.empty-state ul {
  text-align: left;
  display: inline-block;
  margin-top: 0rem;
}

.advice-content :deep(strong),
.message-content :deep(strong) {
  color: #2c3e50;
}

.advice-content :deep(h3),
.message-content :deep(h3) {
  color: #4CAF50;
  margin-top: 1.5rem;
  margin-bottom: 0.5rem;
}

.advice-content :deep(br),
.message-content :deep(br) {
  content: "";
  display: block;
  margin-bottom: 0.8rem;
}

.advice-content :deep(ul),
.message-content :deep(ul) {
  padding-left: 1.5rem;
  margin: 1rem 0;
}

.advice-content :deep(li),
.message-content :deep(li) {
  margin-bottom: 0.5rem;
  position: relative;
  padding-left: 1.5rem;
}

.advice-content :deep(li):before,
.message-content :deep(li):before {
  content: "•";
  color: #4CAF50;
  font-weight: bold;
  position: absolute;
  left: 0;
}
</style>
