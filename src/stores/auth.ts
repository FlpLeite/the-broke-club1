import { defineStore } from 'pinia'
import axios from 'axios'

interface User {
  idUsuario: number;
  nome: string;
  email: string;
  senhaHash: string;
  dataCriacao: string;
}

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null as User | null, 
  }),

  getters: {
    isAuthenticated: (state) => !!state.user
  },

  actions: {
    async signup(nome: string, email: string, senhaHash: string) {
      const response = await axios.post('http://localhost:5024/usuarios', {
        nome,
        email,
        senhaHash
      })

      this.user = response.data
    },

    async login(email: string, senha: string) {
      const response = await axios.post('http://localhost:5024/usuarios/login', {
        email,
        senhaHash: senha
      })

      this.user = response.data
    }
  }
})
