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

    <div v-if="activeTab === 'analysis'" class="content-card">
      <!-- Seletor de Nível de Análise -->
      <div class="analysis-level-selector">
        <h3><i class="fas fa-layer-group"></i> Nível de Análise</h3>
        <p class="selector-description">
          Escolha a profundidade da análise conforme suas necessidades
        </p>
        
        <div class="level-cards">
          <div
            v-for="level in ANALYSIS_LEVELS"
            :key="level.id"
            :class="['level-card', { active: selectedLevel.id === level.id }]"
            @click="selectedLevel = level"
          >
            <div class="level-icon">
              <i :class="level.icon"></i>
            </div>
            <div class="level-info">
              <h4>{{ level.name }}</h4>
              <p>{{ level.description }}</p>
            </div>
            <div class="level-badge" :class="level.id">
              {{ level.id.toUpperCase() }}
            </div>
          </div>
        </div>

        <div class="level-features">
          <h4>Recursos incluídos:</h4>
          <ul>
            <li v-if="selectedLevel.id === 'bronze'">
              <i class="fas fa-check"></i> Resumo executivo
              <i class="fas fa-check"></i> Principais observações
              <i class="fas fa-check"></i> Dica prática rápida
            </li>
            <li v-if="selectedLevel.id === 'prata'">
              <i class="fas fa-check"></i> Panorama financeiro completo
              <i class="fas fa-check"></i> Análise de padrões detalhada
              <i class="fas fa-check"></i> Recomendações práticas
              <i class="fas fa-check"></i> Saúde financeira básica
            </li>
            <li v-if="selectedLevel.id === 'ouro'">
              <i class="fas fa-check"></i> Diagnóstico completo
              <i class="fas fa-check"></i> Estratégias de otimização
              <i class="fas fa-check"></i> Orçamento personalizado
              <i class="fas fa-check"></i> Metas SMART
              <i class="fas fa-check"></i> Saúde financeira avançada
            </li>
          </ul>
        </div>
      </div>

      <!-- Controles de Análise -->
      <div class="analysis-controls">
        <button
          @click="analyzeFinances"
          :disabled="isLoading || !hasTransactions"
          class="analyze-button"
          :class="selectedLevel.id"
        >
          <span v-if="!isLoading">
            <i class="fas fa-magic"></i> Gerar Análise {{ selectedLevel.name }}
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
        <div class="analysis-header" :class="selectedLevel.id">
          <h3>
            <i :class="selectedLevel.icon"></i>
            Análise {{ selectedLevel.name }}
          </h3>
          <span class="analysis-badge">{{ selectedLevel.id.toUpperCase() }}</span>
        </div>
        <div class="advice-content" v-html="formattedAdvice"></div>
      </div>

      <div v-else-if="!isLoading" class="empty-state">
        <div class="empty-image-container">
          <img src="../assets/Penny.png" alt="Penny - Assistente Financeira" class="empty-image">
        </div>
        <strong><h1 class="Apresentacao">Olá, me chamo Penny!</h1></strong>
        <p>Escolha um nível de análise acima para obter insights personalizados sobre sua situação financeira</p>
        
        <div class="level-comparison">
          <h4>Como escolher o nível ideal:</h4>
          <ul>
            <li><strong>Bronze:</strong> Ideal para uma visão geral rápida</li>
            <li><strong>Prata:</strong> Perfeito para otimização mensal</li>
            <li><strong>Ouro:</strong> Recomendado para planejamento estratégico</li>
          </ul>
        </div>
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
import { geminiService, ANALYSIS_LEVELS, type AnalysisLevel, type AnalysisConfig } from '../services/iaService';
import pennyAvatar from '../assets/Penny.png';
import userAvatar from '../assets/user.svg';

const authStore = useAuthStore();
const transactionsStore = useTransactionsStore();
const router = useRouter();

// Variáveis para a aba de análise
const isLoading = ref(false);
const error = ref('');
const advice = ref('');
const activeTab = ref<'analysis' | 'chat'>('analysis');
const selectedLevel = ref<AnalysisLevel>(ANALYSIS_LEVELS[0]); // Bronze padrão

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
  'Quais são meus maiores gastos este mês?',
  'Como posso economizar mais dinheiro?',
  'Estou gastando muito com alimentação?',
  'Quais categorias posso reduzir meus gastos?',
  'Como está meu saldo atual?'
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

    const config: AnalysisConfig = {
      level: selectedLevel.value,
      includePatterns: true,
      includeRecommendations: true,
      includeBudget: selectedLevel.value.id !== 'bronze',
      includeGoals: selectedLevel.value.id === 'ouro',
      includeHealth: true
    };

    const timeout = new Promise((_, reject) =>
      setTimeout(() => reject(new Error('Tempo excedido na análise')), 30000)
    );

    advice.value = await Promise.race([
      geminiService.getFinancialAdvice(transactionsStore.transactions, config),
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

/* ===== Renderizador de Markdown (sem libs) ===== */
const escapeHtml = (str: string) =>
  str
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;');

const renderMarkdown = (md: string) => {
  if (!md) return '';

  // 1) Preserva blocos de código ```...```
  const codeBlocks: string[] = [];
  md = md.replace(/```([\s\S]*?)```/g, (_, code) => {
    const i = codeBlocks.push(`<pre><code>${escapeHtml(code.trim())}</code></pre>`) - 1;
    return `@@CODEBLOCK_${i}@@`;
  });

  // 2) Escapa HTML restante
  md = escapeHtml(md);

  // 3) Inline code `code`
  md = md.replace(/`([^`]+)`/g, '<code>$1</code>');

  // 4) Títulos
  md = md
    .replace(/^###### (.*)$/gm, '<h6>$1</h6>')
    .replace(/^##### (.*)$/gm, '<h5>$1</h5>')
    .replace(/^#### (.*)$/gm, '<h4>$1</h4>')
    .replace(/^### (.*)$/gm, '<h3>$1</h3>')
    .replace(/^## (.*)$/gm, '<h2>$1</h2>')
    .replace(/^# (.*)$/gm, '<h1>$1</h1>');

  // 5) Negrito e itálico
  md = md
    .replace(/\*\*(.+?)\*\*/g, '<strong>$1</strong>')
    .replace(/\*(.+?)\*/g, '<em>$1</em>')
    .replace(/__(.+?)__/g, '<strong>$1</strong>')
    .replace(/_(.+?)_/g, '<em>$1</em>');

  // 6) Links [texto](url)
  md = md.replace(
    /\[([^\]]+)\]\((https?:\/\/[^\s)]+)\)/g,
    `<a href="$2" target="_blank" rel="noopener noreferrer">$1</a>`
  );

  // 7) Listas (ul/ol) por linhas
  const lines = md.split(/\r?\n/);
  let html = '';
  let inUl = false;
  let inOl = false;

  const closeLists = () => {
    if (inUl) { html += '</ul>'; inUl = false; }
    if (inOl) { html += '</ol>'; inOl = false; }
  };

  for (const rawLine of lines) {
    const line = rawLine.trim();

    if (/^- (.+)/.test(line)) {
      if (!inUl) { closeLists(); html += '<ul>'; inUl = true; }
      html += `<li>${line.replace(/^- (.+)/, '$1')}</li>`;
      continue;
    }
    if (/^\d+\. (.+)/.test(line)) {
      if (!inOl) { closeLists(); html += '<ol>'; inOl = true; }
      html += `<li>${line.replace(/^\d+\. (.+)/, '$1')}</li>`;
      continue;
    }

    if (line === '') { // linha vazia fecha listas
      closeLists();
      continue;
    }

    // Parágrafo comum (se não for heading/codeblock)
    if (!/^<h\d>/.test(line) && !/^@@CODEBLOCK_\d+@@$/.test(line)) {
      closeLists();
      html += `<p>${line}</p>`;
    } else {
      closeLists();
      html += rawLine; // já é HTML pronto
    }
  }
  closeLists();

  // 8) Recoloca os codeblocks
  html = html.replace(/@@CODEBLOCK_(\d+)@@/g, (_, i) => codeBlocks[Number(i)]);

  return html;
};

// Usa o renderizador no chat e no advice
const formatMessage = (content: string) => renderMarkdown(content);
const formattedAdvice = computed(() => renderMarkdown(advice.value || ''));
/* ===== Fim do renderizador ===== */

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
  color: #0062a3;
  border-bottom-color: #4c90af;
  font-weight: 600;
}

.content-card {
  background-color: white;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  padding: 2rem;
}

.dark .content-card {
  background-color: #121C2A;
  color: #F3F7FA;
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

.dark .chat-messages {
  background-color: #1F2937;
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

.dark .message.user {
  background-color: rgba(37, 99, 235, 0.3);
}

.message.assistant {
  margin-right: auto;
  background-color: #f1f1f1;
  border-bottom-left-radius: 0;
}

.dark .message.assistant {
  background-color: #374151;
}

.message-header {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 0.5rem;
  font-size: 0.85rem;
  color: #9c9c9c;
}

.message-time {
  font-size: 0.75rem;
  opacity: 0.7;
}

.message-content {
  line-height: 1.5;
}

/* ===== Ajustes de Markdown ===== */

/* Negrito deve herdar a cor do tema, não ficar fixo escuro */
.advice-content :deep(strong),
.message-content :deep(strong) {
  color: inherit;
  font-weight: 700;
}

/* Títulos renderizados a partir de # ## ### */
.message-content :deep(h1),
.message-content :deep(h2),
.message-content :deep(h3),
.advice-content :deep(h1),
.advice-content :deep(h2),
.advice-content :deep(h3) {
  margin: 0.75rem 0 0.5rem;
  line-height: 1.2;
}

.message-content :deep(h1),
.advice-content :deep(h1) { font-size: 1.4rem; }

.message-content :deep(h2),
.advice-content :deep(h2) { font-size: 1.2rem; }

.message-content :deep(h3),
.advice-content :deep(h3) { font-size: 1.05rem; }

/* Cores dos títulos no dark */
.dark .message-content :deep(h1),
.dark .message-content :deep(h2),
.dark .message-content :deep(h3),
.dark .advice-content :deep(h1),
.dark .advice-content :deep(h2),
.dark .advice-content :deep(h3) {
  color: #F3F7FA;
}

/* Links seguros e visíveis */
.message-content :deep(a),
.advice-content :deep(a) {
  text-decoration: underline;
  word-break: break-word;
}

/* Parágrafos e quebra de linha */
.advice-content :deep(br),
.message-content :deep(br) {
  content: "";
  display: block;
  margin-bottom: 0.8rem;
}

/* Listas */
.advice-content :deep(ul),
.message-content :deep(ul),
.advice-content :deep(ol),
.message-content :deep(ol) {
  padding-left: 1.5rem;
  margin: 1rem 0;
}

.advice-content :deep(li),
.message-content :deep(li) {
  margin-bottom: 0.5rem;
  position: relative;
  padding-left: 1.5rem;
}

.advice-content :deep(ul) :deep(li)::before,
.message-content :deep(ul) :deep(li)::before {
  content: "•";
  color: #4CAF50;
  font-weight: bold;
  position: absolute;
  left: 0;
}

/* Blocos e inline-code */
.message-content :deep(pre),
.advice-content :deep(pre) {
  background: rgba(0,0,0,0.05);
  padding: 0.75rem 1rem;
  border-radius: 6px;
  overflow-x: auto;
}

.dark .message-content :deep(pre),
.dark .advice-content :deep(pre) {
  background: rgba(255,255,255,0.07);
}

.message-content :deep(code),
.advice-content :deep(code) {
  font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, "Liberation Mono", "Courier New", monospace;
  padding: 0.1rem 0.3rem;
  border-radius: 4px;
  background: rgba(0,0,0,0.06);
}

.dark .message-content :deep(code),
.dark .advice-content :deep(code) {
  background: rgba(255,255,255,0.12);
}

/* ===== Fim ajustes Markdown ===== */

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

.dark .chat-input {
  background-color: #1F2937;
  border-color: #4B5563;
  color: #F3F7FA;
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

.dark .suggestion-button {
  background-color: #374151;
  border-color: #4B5563;
  color: #E5E7EB;
}

.suggestion-button:hover {
  background-color: #e0e0e0;
}

.dark .suggestion-button:hover {
  background-color: #4B5563;
}

/* ===== Estilos para o Seletor de Níveis ===== */
.analysis-level-selector {
  margin-bottom: 2rem;
  padding: 1.5rem;
  background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
  border-radius: 12px;
  border: 1px solid #e0e0e0;
}

.dark .analysis-level-selector {
  background: linear-gradient(135deg, #1F2937 0%, #374151 100%);
  border-color: #4B5563;
}

.selector-description {
  color: #6c757d;
  margin-bottom: 1.5rem;
  font-size: 0.95rem;
}

.level-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.level-card {
  background: white;
  border: 2px solid #e0e0e0;
  border-radius: 12px;
  padding: 1.5rem;
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}

.dark .level-card {
  background: #1F2937;
  border-color: #4B5563;
}

.level-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.level-card.active {
  border-color: #4CAF50;
  background: linear-gradient(135deg, #f0f9f0 0%, #e8f5e8 100%);
}

.dark .level-card.active {
  background: linear-gradient(135deg, #1B3B1B 0%, #1A331A 100%);
}

.level-card.bronze.active {
  border-color: #CD7F32;
  background: linear-gradient(135deg, #fdf6f0 0%, #fcefe6 100%);
}

.level-card.prata.active {
  border-color: #C0C0C0;
  background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
}

.level-card.ouro.active {
  border-color: #FFD700;
  background: linear-gradient(135deg, #fff9e6 0%, #fff3cc 100%);
}

.level-icon {
  font-size: 2rem;
  margin-bottom: 1rem;
  color: #6c757d;
}

.level-card.active .level-icon {
  color: #4CAF50;
}

.level-card.bronze.active .level-icon {
  color: #CD7F32;
}

.level-card.prata.active .level-icon {
  color: #C0C0C0;
}

.level-card.ouro.active .level-icon {
  color: #FFD700;
}

.level-info h4 {
  margin: 0 0 0.5rem 0;
  font-size: 1.2rem;
  font-weight: 600;
}

.level-info p {
  margin: 0;
  color: #6c757d;
  font-size: 0.9rem;
}

.level-badge {
  position: absolute;
  top: 1rem;
  right: 1rem;
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
}

.level-badge.bronze {
  background: #CD7F32;
  color: white;
}

.level-badge.prata {
  background: #C0C0C0;
  color: #333;
}

.level-badge.ouro {
  background: #FFD700;
  color: #333;
}

.level-features {
  background: white;
  padding: 1rem;
  border-radius: 8px;
  border: 1px solid #e0e0e0;
}

.dark .level-features {
  background: #1F2937;
  border-color: #4B5563;
}

.level-features h4 {
  margin-bottom: 0.5rem;
  font-size: 1rem;
}

.level-features ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.level-features li {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.25rem;
  font-size: 0.9rem;
}

.level-features i.fa-check {
  color: #4CAF50;
}

.analysis-controls {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-bottom: 1.5rem;
}

.analyze-button {
  background-color: #368dff;
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

.analyze-button.bronze {
  background-color: #CD7F32;
}

.analyze-button.prata {
  background-color: #C0C0C0;
  color: #333;
}

.analyze-button.ouro {
  background-color: #FFD700;
  color: #333;
}

.analyze-button:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

.analyze-button.bronze:hover:not(:disabled) {
  background-color: #b56a28;
}

.analyze-button.prata:hover:not(:disabled) {
  background-color: #a8a8a8;
}

.analyze-button.ouro:hover:not(:disabled) {
  background-color: #e6c200;
}

.analyze-button:disabled {
  background-color: #b0bec5;
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
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

.analysis-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  padding-bottom: 1rem;
  border-bottom: 2px solid #e0e0e0;
}

.analysis-header.bronze {
  border-bottom-color: #CD7F32;
}

.analysis-header.prata {
  border-bottom-color: #C0C0C0;
}

.analysis-header.ouro {
  border-bottom-color: #FFD700;
}

.analysis-header h3 {
  margin: 0;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.analysis-badge {
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
}

.analysis-badge.bronze {
  background: #CD7F32;
  color: white;
}

.analysis-badge.prata {
  background: #C0C0C0;
  color: #333;
}

.analysis-badge.ouro {
  background: #FFD700;
  color: #333;
}

.advice-content {
  line-height: 1.8;
  padding: 1rem;
  background-color: #f9f9f9;
  border-radius: 8px;
  border-left: 4px solid #4CAF50;
}

.dark .advice-content {
  background-color: #1F2937;
  color: #F3F7FA;
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
  border: 4px solid #00fff2;
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

.level-comparison {
  margin-top: 2rem;
  padding: 1.5rem;
  background: #f8f9fa;
  border-radius: 8px;
}

.dark .level-comparison {
  background: #1F2937;
}

.level-comparison h4 {
  margin-bottom: 1rem;
  color: #2c3e50;
}

.dark .level-comparison h4 {
  color: #F3F7FA;
}

.level-comparison ul {
  text-align: left;
  max-width: 500px;
  margin: 0 auto;
}

.level-comparison li {
  margin-bottom: 0.5rem;
  padding-left: 1rem;
}

.empty-state ul {
  text-align: left;
  display: inline-block;
  margin-top: 0rem;
}
</style>