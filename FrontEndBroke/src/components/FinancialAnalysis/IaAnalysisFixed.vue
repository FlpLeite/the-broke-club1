<template>
  <div class="content-card">
    <!-- Seletor de Nível de Análise (agora em abas) -->
    <div class="analysis-level-selector">
      <h3><i class="fas fa-layer-group"></i> Nível de Análise</h3>
      <p class="selector-description">
        Escolha a profundidade da análise conforme suas necessidades
      </p>

      <!-- Abas: ícone + nome -->
      <div class="level-tabs" role="tablist" aria-label="Níveis de análise">
        <button
          v-for="level in ANALYSIS_LEVELS"
          :key="level.id"
          role="tab"
          :aria-selected="selectedLevel.id === level.id"
          :class="['level-tab', { active: selectedLevel.id === level.id }]"
          @click="selectedLevel = level"
        >
          <i :class="level.icon"></i>
          <span>{{ level.name }}</span>
        </button>
      </div>

      <!-- Descrição do nível selecionado -->
      <p class="level-description">{{ selectedLevel.description }}</p>

      <!-- Recursos incluídos (lista limpa) -->
      <div class="level-features">
        <h4>Recursos incluídos:</h4>
        <ul>
          <!-- Bronze -->
          <li v-if="selectedLevel.id === 'bronze'"><i class="fas fa-check"></i> Resumo executivo</li>
          <li v-if="selectedLevel.id === 'bronze'"><i class="fas fa-check"></i> Principais observações</li>
          <li v-if="selectedLevel.id === 'bronze'"><i class="fas fa-check"></i> Dica prática rápida</li>

          <!-- Prata -->
          <li v-if="selectedLevel.id === 'prata'"><i class="fas fa-check"></i> Panorama financeiro completo</li>
          <li v-if="selectedLevel.id === 'prata'"><i class="fas fa-check"></i> Análise de padrões detalhada</li>
          <li v-if="selectedLevel.id === 'prata'"><i class="fas fa-check"></i> Recomendações práticas</li>
          <li v-if="selectedLevel.id === 'prata'"><i class="fas fa-check"></i> Saúde financeira básica</li>

          <!-- Ouro -->
          <li v-if="selectedLevel.id === 'ouro'"><i class="fas fa-check"></i> Diagnóstico completo</li>
          <li v-if="selectedLevel.id === 'ouro'"><i class="fas fa-check"></i> Estratégias de otimização</li>
          <li v-if="selectedLevel.id === 'ouro'"><i class="fas fa-check"></i> Orçamento personalizado</li>
          <li v-if="selectedLevel.id === 'ouro'"><i class="fas fa-check"></i> Metas SMART</li>
          <li v-if="selectedLevel.id === 'ouro'"><i class="fas fa-check"></i> Saúde financeira avançada</li>
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
        <img src="../../assets/Penny2.png" alt="Penny - Assistente Financeira" class="empty-image">
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
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '../../stores/auth';
import { useTransactionsStore } from '../../stores/transactions';
import { geminiService, ANALYSIS_LEVELS, type AnalysisLevel, type AnalysisConfig } from '../../services/iaService';

// Props
defineProps<{
  hasTransactions: boolean;
}>();

const authStore = useAuthStore();
const transactionsStore = useTransactionsStore();
const router = useRouter();

// Variáveis para a análise
const isLoading = ref(false);
const error = ref('');
const advice = ref('');
const selectedLevel = ref<AnalysisLevel>(ANALYSIS_LEVELS[0]); // Bronze padrão

const analyzeFinances = async () => {
  if (!authStore.isAuthenticated) {
    await router.push({ name: 'login' });
    return;
  }

  isLoading.value = true;
  error.value = '';
  advice.value = '';

  try {
    const config: AnalysisConfig = {
      level: selectedLevel.value,
      includePatterns: true,
      includeRecommendations: true,
      includeBudget: selectedLevel.value.id !== 'bronze',
      includeGoals: selectedLevel.value.id === 'ouro',
      includeHealth: true
    };

    const timeout: Promise<never> = new Promise((_, reject) =>
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

/* ===== Renderizador de Markdown ===== */
const escapeHtml = (str: string) =>
  str
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;');

const renderMarkdown = (md: string) => {
  if (!md) return '';

  // Preserva blocos de código
  const codeBlocks: string[] = [];
  md = md.replace(/```([\s\S]*?)```/g, (_, code) => {
    const i = codeBlocks.push(`<pre><code>${escapeHtml(code.trim())}</code></pre>`) - 1;
    return `@@CODEBLOCK_${i}@@`;
  });

  // Escapa HTML
  md = escapeHtml(md);

  // Inline code
  md = md.replace(/`([^`]+)`/g, '<code>$1</code>');

  // Títulos
  md = md
    .replace(/^###### (.*)$/gm, '<h6>$1</h6>')
    .replace(/^##### (.*)$/gm, '<h5>$1</h5>')
    .replace(/^#### (.*)$/gm, '<h4>$1</h4>')
    .replace(/^### (.*)$/gm, '<h3>$1</h3>')
    .replace(/^## (.*)$/gm, '<h2>$1</h2>')
    .replace(/^# (.*)$/gm, '<h1>$1</h1>');

  // Negrito e itálico
  md = md
    .replace(/\*\*(.+?)\*\*/g, '<strong>$1</strong>')
    .replace(/\*(.+?)\*/g, '<em>$1</em>')
    .replace(/__(.+?)__/g, '<strong>$1</strong>')
    .replace(/_(.+?)_/g, '<em>$1</em>');

  // Links
  md = md.replace(
    /\[([^\]]+)\]\((https?:\/\/[^\s)]+)\)/g,
    `<a href="$2" target="_blank" rel="noopener noreferrer">$1</a>`
  );

  // Processamento de listas
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

    if (line === '') {
      closeLists();
      continue;
    }

    if (!/^<h\d>/.test(line) && !/^@@CODEBLOCK_\d+@@$/.test(line)) {
      closeLists();
      html += `<p>${line}</p>`;
    } else {
      closeLists();
      html += rawLine;
    }
  }
  closeLists();

  // Recoloca os codeblocks
  html = html.replace(/@@CODEBLOCK_(\d+)@@/g, (_, i) => codeBlocks[Number(i)]);

  return html;
};

const formattedAdvice = computed(() => renderMarkdown(advice.value || ''));
</script>

<style scoped>
.content-card {
  background-color: #0091aa;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  padding: 2rem;
}

.dark .content-card {
  background-color: #121C2A;
  color: #F3F7FA;
}

.Apresentacao {
  font-size: 140%;
}

/* ===== Estilos para o Seletor de Níveis (atualizado para abas) ===== */
.analysis-level-selector {
  margin-bottom: 2rem;
  padding: 1.5rem;
  background: linear-gradient(135deg, #225385 0%, #0958a7 100%);
  border-radius: 12px;
  border: 1px solid #e0e0e0;
}

.dark .analysis-level-selector {
  background: linear-gradient(135deg, #1F2937 0%, #374151 100%);
  border-color: #4B5563;
}

.selector-description {
  color: #6c757d;
  margin-bottom: 1rem;
  font-size: 0.95rem;
}

/* Abas */
.level-tabs {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
  margin-bottom: 1rem;
}

.level-tab {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem .9rem;
  border: 1px solid #e0e0e0;
  border-radius: 10px;
  background:#213049;
  font-weight: 600;
  cursor: pointer;
  transition: all .2s ease;
}

.level-tab i { font-size: 1rem; }

.level-tab:hover {
  transform: translateY(-1px);
  box-shadow: 0 4px 10px rgba(0,0,0,.06);
}

.level-tab.active {
  border-color: #02e2ff;
background: linear-gradient(135deg, #002d4b 0%, #0b3477 100%);
  box-shadow: 0 4px 10px rgba(0,0,0,.08);
}

/* Descrição sob as abas */
.level-description {
  margin: 0 0 1rem 0;
  color: #6c757d;
  font-size: 0.95rem;
}

/* Recursos incluídos (lista) */
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
  color: #36f8ff;
}

/* ===== Controles de Análise ===== */
.analysis-controls {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-bottom: 1.5rem;
}

.analyze-button {
  background-color: #2e4b50;
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

/* ===== Erros ===== */
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

.dark .error-message {
  background-color: rgba(231, 76, 60, 0.1);
}

/* ===== Resultado ===== */
.advice-container {
  margin-top: 2rem;
  border-top: 1px solid #eee;
  padding-top: 1.5rem;
}

.dark .advice-container {
  border-top-color: #374151;
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
  border-left: 4px solid #31ffff;
}

.dark .advice-content {
  background-color: #1F2937;
  color: #F3F7FA;
}

/* ===== Empty state ===== */
.empty-state {
  text-align: center;
  padding: 2rem 0;
}

.empty-image-container {
  display: flex;
  justify-content: center; /* centraliza horizontalmente */
  align-items: center;     /* centraliza verticalmente dentro do container */
  margin-bottom: 1.5rem;   /* dá um respiro abaixo da imagem */
}

.empty-image {
  width: 185px;
  height: 185px;
  object-position: 0 -28px;
  border-radius: 50%;
  image-rendering: auto;
  object-fit: cover;
  border: 4px solid #00fff2;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.empty-state h3 {
  color: #2c3e50;
  margin-bottom: 1rem;
}

.dark .empty-state h3 {
  color: #F3F7FA;
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

/* ===== Ajustes de Markdown ===== */
.advice-content :deep(strong) {
  color: inherit;
  font-weight: 700;
}

.advice-content :deep(h1),
.advice-content :deep(h2),
.advice-content :deep(h3) {
  margin: 0.75rem 0 0.5rem;
  line-height: 1.2;
}

.advice-content :deep(h1) { font-size: 1.4rem; }
.advice-content :deep(h2) { font-size: 1.2rem; }
.advice-content :deep(h3) { font-size: 1.05rem; }

.dark .advice-content :deep(h1),
.dark .advice-content :deep(h2),
.dark .advice-content :deep(h3) {
  color: #F3F7FA;
}

.advice-content :deep(a) {
  text-decoration: underline;
  word-break: break-word;
}

.advice-content :deep(br) {
  content: "";
  display: block;
  margin-bottom: 0.8rem;
}

.advice-content :deep(ul),
.advice-content :deep(ol) {
  padding-left: 1.5rem;
  margin: 1rem 0;
}

.advice-content :deep(li) {
  margin-bottom: 0.5rem;
  position: relative;
  padding-left: 1.5rem;
}

.advice-content :deep(ul) :deep(li)::before {
  content: "•";
  color: #4CAF50;
  font-weight: bold;
  position: absolute;
  left: 0;
}

.advice-content :deep(pre) {
  background: rgba(0,0,0,0.05);
  padding: 0.75rem 1rem;
  border-radius: 6px;
  overflow-x: auto;
}

.dark .advice-content :deep(pre) {
  background: rgba(255,255,255,0.07);
}

.advice-content :deep(code) {
  font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, "Liberation Mono", "Courier New", monospace;
  padding: 0.1rem 0.3rem;
  border-radius: 4px;
  background: rgba(0,0,0,0.06);
}

.dark .advice-content :deep(code) {
  background: rgba(255,255,255,0.12);
}
</style>
