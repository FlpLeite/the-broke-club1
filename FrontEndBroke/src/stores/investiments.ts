import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import axios from 'axios'
import { useAuthStore } from './auth'

export interface Investment {
    id: string
    name: string
    type: 'stocks' | 'bonds' | 'funds' | 'crypto' | 'real_estate' | 'savings'
    amount: number
    currentValue: number
    purchaseDate: string
    broker: string
    notes?: string
}

const formatDateToISO = (date: string | Date) => {
    const parsedDate = typeof date === 'string' ? new Date(date) : date
    return new Date(parsedDate).toISOString().split('T')[0]
}

const mapFromApi = (investment: any): Investment => ({
    id: investment.idInvestimento.toString(),
    name: investment.nome,
    type: investment.tipo as Investment['type'],
    amount: Number(investment.valorInvestido),
    currentValue: Number(investment.valorAtual),
    purchaseDate: formatDateToISO(investment.dataCompra),
    broker: investment.corretora ?? '',
    notes: investment.notas ?? ''
})


export const useInvestmentsStore = defineStore('investments', () => {
    const investments = ref<Investment[]>([])
    const isLoading = ref(false)
    const error = ref<string | null>(null)

    const investmentTypes = [
        { value: 'stocks', label: 'Ações' },
        { value: 'bonds', label: 'Títulos Públicos' },
        { value: 'funds', label: 'Fundos de Investimento' },
        { value: 'crypto', label: 'Criptomoedas' },
        { value: 'real_estate', label: 'Fundos Imobiliários' },
        { value: 'savings', label: 'Poupança' }
    ]
    const addInvestment = async (investment: Omit<Investment, 'id'>) => {
        try {
            const authStore = useAuthStore()
            const idUsuario = authStore.user?.idUsuario

            if (!idUsuario) {
                throw new Error('Usuário não autenticado.')
            }

            const response = await axios.post('http://localhost:5024/investimentos', {
                idUsuario,
                nome: investment.name,
                tipo: investment.type,
                valorInvestido: investment.amount,
                valorAtual: investment.currentValue,
                dataCompra: new Date(investment.purchaseDate).toISOString(),
                corretora: investment.broker,
                notas: investment.notes
            })

            const novoInvestimento = mapFromApi(response.data)
            investments.value.push(novoInvestimento)
        } catch (err) {
            console.error('Erro ao adicionar investimento:', err)
            throw err
        }
    }

    const updateInvestment = async (id: string, updatedInvestment: Omit<Investment, 'id'>) => {
        try {
            await axios.put(`http://localhost:5024/investimentos/${id}`, {
                nome: updatedInvestment.name,
                tipo: updatedInvestment.type,
                valorInvestido: updatedInvestment.amount,
                valorAtual: updatedInvestment.currentValue,
                dataCompra: new Date(updatedInvestment.purchaseDate).toISOString(),
                corretora: updatedInvestment.broker,
                notas: updatedInvestment.notes
            })

            const index = investments.value.findIndex(i => i.id === id)
            if (index !== -1) {
                investments.value[index] = {
                    ...updatedInvestment,
                    purchaseDate: formatDateToISO(updatedInvestment.purchaseDate),
                    id
                }
            }
        } catch (err) {
            console.error('Erro ao atualizar investimento:', err)
            throw err
        }
    }

    const deleteInvestment = async (id: string) => {
        try {
            await axios.delete(`http://localhost:5024/investimentos/${id}`)
            investments.value = investments.value.filter(i => i.id !== id)
        } catch (err) {
            console.error('Erro ao excluir investimento:', err)
            throw err
        }
    }

    const loadInvestments = async () => {
        try {
            isLoading.value = true
            error.value = null

            const authStore = useAuthStore()
            const idUsuario = authStore.user?.idUsuario

            if (!idUsuario) {
                throw new Error('Usuário não autenticado.')
            }

            const response = await axios.get(`http://localhost:5024/investimentos/usuario/${idUsuario}`)
            investments.value = response.data.map((investment: any) => mapFromApi(investment))
        } catch (err: any) {
            console.error('Erro ao carregar investimentos:', err)
            error.value = err?.message ?? 'Erro ao carregar investimentos.'
        } finally {
            isLoading.value = false
        }
    }

    const totalInvested = computed(() => {
        return investments.value.reduce((sum, investment) => sum + investment.amount, 0)
    })

    const totalCurrentValue = computed(() => {
        return investments.value.reduce((sum, investment) => sum + investment.currentValue, 0)
    })

    const totalReturn = computed(() => {
        return totalCurrentValue.value - totalInvested.value
    })

    const totalReturnPercentage = computed(() => {
        if (totalInvested.value === 0) return 0
        return ((totalReturn.value / totalInvested.value) * 100)
    })

    const investmentsByType = computed(() => {
        const result: Record<string, { amount: number, currentValue: number, count: number }> = {}

        investments.value.forEach(investment => {
            const typeLabel = investmentTypes.find(t => t.value === investment.type)?.label || investment.type

            if (!result[typeLabel]) {
                result[typeLabel] = { amount: 0, currentValue: 0, count: 0 }
            }

            result[typeLabel].amount += investment.amount
            result[typeLabel].currentValue += investment.currentValue
            result[typeLabel].count += 1
        })

        return result
    })

    return {
        investments,
        investmentTypes,
        isLoading,
        error,
        addInvestment,
        updateInvestment,
        deleteInvestment,
        loadInvestments,
        totalInvested,
        totalCurrentValue,
        totalReturn,
        totalReturnPercentage,
        investmentsByType
    }
})