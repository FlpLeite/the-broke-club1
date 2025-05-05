import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import axios from 'axios'
import { useAuthStore } from '../stores/auth'



export interface Transaction {
  id: string
  date: string
  description: string
  category: string
  amount: number
  type: 'income' | 'expense'
}

export const useTransactionsStore = defineStore('transactions', () => {
  const transactions = ref<Transaction[]>([])

  const categories = [
    'Receita',
    'Moradia',
    'Alimentação',
    'Transporte',
    'Lazer',
    'Utilidades',
    'Saúde',
    'Educação',
    'Compras',
    'Pessoal',
    'Investimentos',
    'Outros'
  ]

  const addTransaction = async (transaction: Omit<Transaction, 'id'> & { idUsuario: number }) => {
    try {
      const authStore = useAuthStore()
      const idUsuario = authStore.user?.idUsuario
  
      if (!idUsuario) {
        throw new Error('Usuário não autenticado.')
      }
  
      console.log('Enviando transação:', {
        tipo: transaction.type,
        categoria: transaction.category,
        valor: transaction.amount,
        descricao: transaction.description,
        idUsuario: idUsuario,
      })
  
      const response = await axios.post('http://localhost:5024/transacoes', {
        idUsuario: idUsuario,
        tipo: transaction.type,
        categoria: transaction.category,
        valor: transaction.amount,
        descricao: transaction.description,
        dataTransacao: new Date(transaction.date).toISOString(),
      })

      const novaTransacao = response.data
      transactions.value.push({
        id: novaTransacao.idTransacao.toString(),
        date: new Date(novaTransacao.dataTransacao).toISOString().split('T')[0],
        description: novaTransacao.descricao || '',
        category: novaTransacao.categoria,
        amount: Number(novaTransacao.valor),
        type: novaTransacao.tipo.toLowerCase() === 'income' ? 'income' : 'expense'
      })
    } catch (error) {
      console.error('Erro ao criar transação:', error)
    }
    
  }


  const updateTransaction = async (id: string, updated: Omit<Transaction, 'id'>) => {
    try {
      const authStore = useAuthStore()
      const idUsuario = authStore.user?.idUsuario
      if (!idUsuario) throw new Error('Usuário não autenticado.')
  
      const payload = {
        IdTransacao:   Number(id),
        IdUsuario:     idUsuario,
        Tipo:          updated.type,
        Categoria:     updated.category,
        Valor:         updated.amount,
        Descricao:     updated.description,
        DataTransacao: new Date(updated.date).toISOString()
      }
  
      await axios.put(`http://localhost:5024/transacoes/${id}`, payload)
      
      await loadTransactions()
    } catch (error) {
      console.error('Erro ao atualizar transação:', error)
    }
  }
  
  
  

  const deleteTransaction = async (id: string) => {
    try {
      const authStore = useAuthStore()
      const idUsuario = authStore.user?.idUsuario
      if (!idUsuario) throw new Error('Usuário não autenticado.')
  
      await axios.delete(`http://localhost:5024/transacoes/${id}`)
  
      transactions.value = transactions.value.filter(t => t.id !== id)
    } catch (error) {
      console.error('Erro ao deletar transação:', error)
    }
  }

  const totalIncome = computed(() => {
    return transactions.value
      .filter(t => t.type === 'income')
      .reduce((sum, t) => sum + t.amount, 0)
  })

  const totalExpenses = computed(() => {
    return transactions.value
      .filter(t => t.type === 'expense')
      .reduce((sum, t) => sum + t.amount, 0)
  })

  const balance = computed(() => {
    return totalIncome.value - totalExpenses.value
  })

  const expensesByCategory = computed(() => {
    const result: Record<string, number> = {}
    
    transactions.value
      .filter(t => t.type === 'expense')
      .forEach(t => {
        if (!result[t.category]) {
          result[t.category] = 0
        }
        result[t.category] += t.amount
      })
      
    return result
  })

  const loadTransactions = async () => {
    try {
      const authStore = useAuthStore()
      const idUsuario = authStore.user?.idUsuario
  
      if (!idUsuario) {
        throw new Error('Usuário não autenticado.')
      }
  
      const response = await axios.get(`http://localhost:5024/transacoes/usuario/${idUsuario}`)
      transactions.value = response.data.map((t: any) => ({
        id: t.idTransacao.toString(),
        date: new Date(t.dataTransacao).toISOString().split('T')[0],
        description: t.descricao || '',
        category: t.categoria,
        amount: Number(t.valor),
        type: t.tipo.toLowerCase() === 'income' ? 'income' : 'expense'
      }))
    } catch (error) {
      console.error('Erro ao carregar transações:', error)
    }
  }  

  return {
    transactions,
    categories,
    addTransaction,
    updateTransaction,
    deleteTransaction,
    totalIncome,
    totalExpenses,
    balance,
    expensesByCategory,
    loadTransactions
  }
})