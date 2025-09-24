import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

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

export const useInvestmentsStore = defineStore('investments', () => {
    const investments = ref<Investment[]>([
        {
            id: '1',
            name: 'ITUB4',
            type: 'stocks',
            amount: 1000,
            currentValue: 1150,
            purchaseDate: '2024-12-01',
            broker: 'XP Investimentos',
            notes: 'Ações do Itaú Unibanco'
        },
        {
            id: '2',
            name: 'Tesouro Selic 2029',
            type: 'bonds',
            amount: 2000,
            currentValue: 2080,
            purchaseDate: '2024-11-15',
            broker: 'Tesouro Direto',
            notes: 'Título público indexado à Selic'
        },
        {
            id: '3',
            name: 'Bitcoin',
            type: 'crypto',
            amount: 500,
            currentValue: 620,
            purchaseDate: '2024-10-20',
            broker: 'Binance',
            notes: 'Criptomoeda'
        },
        {
            id: '4',
            name: 'Fundo Imobiliário HGLG11',
            type: 'real_estate',
            amount: 1500,
            currentValue: 1420,
            purchaseDate: '2024-09-10',
            broker: 'Rico',
            notes: 'Fundo de investimento imobiliário'
        }
    ])

    const investmentTypes = [
        { value: 'stocks', label: 'Ações' },
        { value: 'bonds', label: 'Títulos Públicos' },
        { value: 'funds', label: 'Fundos de Investimento' },
        { value: 'crypto', label: 'Criptomoedas' },
        { value: 'real_estate', label: 'Fundos Imobiliários' },
        { value: 'savings', label: 'Poupança' }
    ]

    const addInvestment = (investment: Omit<Investment, 'id'>) => {
        const newInvestment = {
            ...investment,
            id: Date.now().toString()
        }
        investments.value.push(newInvestment)
    }

    const updateInvestment = (id: string, updatedInvestment: Omit<Investment, 'id'>) => {
        const index = investments.value.findIndex(i => i.id === id)
        if (index !== -1) {
            investments.value[index] = { ...updatedInvestment, id }
        }
    }

    const deleteInvestment = (id: string) => {
        investments.value = investments.value.filter(i => i.id !== id)
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
        addInvestment,
        updateInvestment,
        deleteInvestment,
        totalInvested,
        totalCurrentValue,
        totalReturn,
        totalReturnPercentage,
        investmentsByType
    }
})