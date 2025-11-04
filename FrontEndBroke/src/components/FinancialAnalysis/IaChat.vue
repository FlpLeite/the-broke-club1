<template>
    <!-- Penny no topo -->
    <div class="penny-header">
        <img :src="pennyAvatar" alt="Penny - Assistente Financeira" class="penny-image" />
    </div>

    <!-- Chat -->
    <div class="content-card chat-container">
        <div class="chat-messages" ref="chatContainer">
            <div v-for="(message, index) in chatMessages" :key="index" :class="['message', message.role]">
                <div class="message-header">
                    <!-- Avatar só do usuário -->
                    <img v-if="message.role === 'user'" :src="userAvatar" alt="Seu avatar"
                        class="empty-image-profile" />
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
                    <input v-model="userMessage" type="text" placeholder="Pergunte algo sobre suas finanças..."
                        :disabled="isChatLoading || !hasTransactions" class="chat-input" /> 
                    <button type="submit" :disabled="!userMessage || isChatLoading || !hasTransactions" 
                        class="send-button">
                        <i class="fa-solid fa-paper-plane"></i>
                    </button>
                </div>

                <div v-if="!hasTransactions" class="warning-message">
                    <i class="fas fa-exclamation-triangle"></i> Adicione transações para habilitar o chat
                </div>
            </form>

            <div class="suggestions">
                <p>Tente perguntar:</p>
                <button v-for="(suggestion, index) in suggestedQuestions" :key="index"
                    @click="userMessage = suggestion; sendMessage()" class="suggestion-button">
                    {{ suggestion }}
                </button>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted, nextTick } from 'vue';
import { useTransactionsStore } from '../../stores/transactions';
import { geminiService } from '../../services/iaService';
import pennyAvatar from '../../assets/teste.png';
import userAvatar from '../../assets/user.svg';

defineProps<{ hasTransactions: boolean }>();

const transactionsStore = useTransactionsStore();
const chatMessages = ref<Array<{ role: 'user' | 'assistant'; content: string; timestamp: Date }>>([]);
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

const sendMessage = async () => {
    if (!userMessage.value.trim() || isChatLoading.value) return;

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
        const response = await geminiService.chatWithAI({
            message,
            transactions: transactionsStore.transactions,
            chatHistory: chatMessages.value.slice(0, -1)
        });

        chatMessages.value.push({
            role: 'assistant',
            content: response,
            timestamp: new Date()
        });
    } catch (err) {
        console.error('Erro no chat:', err);
        chatMessages.value.push({
            role: 'assistant',
            content: 'Desculpe, ocorreu um erro ao processar sua mensagem. Por favor, tente novamente.',
            timestamp: new Date()
        });
    } finally {
        isChatLoading.value = false;
        scrollToBottom();
    }
};

const escapeHtml = (str: string) =>
    str.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');

const renderMarkdown = (md: string) => {
    if (!md) return '';
    return escapeHtml(md)
        .replace(/\*\*(.+?)\*\*/g, '<strong>$1</strong>')
        .replace(/\*(.+?)\*/g, '<em>$1</em>')
        .replace(/__(.+?)__/g, '<strong>$1</strong>')
        .replace(/_(.+?)_/g, '<em>$1</em>')
        .replace(/\[([^\]]+)\]\((https?:\/\/[^\s)]+)\)/g, '<a href="$2" target="_blank" rel="noopener noreferrer">$1</a>');
};

const formatMessage = (content: string) => renderMarkdown(content);

const formatTime = (date: Date) => date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

const scrollToBottom = () => {
    nextTick(() => {
        if (chatContainer.value) chatContainer.value.scrollTop = chatContainer.value.scrollHeight;
    });
};

onMounted(() => {
    chatMessages.value.push({
        role: 'assistant',
        content: 'Olá! Eu sou a Penny, sua assistente financeira. Como posso te ajudar hoje?',
        timestamp: new Date()
    });
    scrollToBottom();
});
</script>

<style scoped>
/* --- Cabeçalho com a Penny --- */
.penny-header {
    display: flex;
    justify-content: center;
    align-items: center;
    margin-bottom: 16px;
}

.penny-image {
    width: 110px;
    height: 110px;
    border-radius: 50%;
    object-fit: cover;
    image-rendering: auto;
    border: 4px solid #02b3ca;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
}

/* --- Chat --- */
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

.empty-image-profile {
    width: 42px;
    height: 42px;
    border-radius: 50%;
    object-fit: cover;
    image-rendering: auto;
    border: 3px solid #02b3ca;
    box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
}

/* --- Input --- */
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
    border-color: #02b3ca;
    box-shadow: 0 0 0 2px rgba(76, 175, 80, 0.2);
}

.send-button {
    width: 48px;
    height: 48px;
    border-radius: 50%;
    border: none;
    background-color: #25e4fd;
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

/* --- Sugestões --- */
.suggestions {
    margin-top: 1rem;
}

.suggestions p {
    font-size: 0.9rem;
    color: #7f8c8d;
    margin-bottom: 0.5rem;
}

.suggestion-button {
    background-color: #1b1b1b;
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

.warning-message {
    margin-top: 1rem;
    color: #ff9800;
    display: flex;
    align-items: center;
    gap: 8px;
}
</style>
