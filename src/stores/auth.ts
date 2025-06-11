import { defineStore } from 'pinia'
import axios from 'axios'

interface User {
  idUsuario: number;
  nome: string;
  email: string;
  senha: string;
  dataCriacao: string;
}

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null as User | null,
    token: null as string | null
  }),

  getters: {
    isAuthenticated: (state) => !!state.user && !!state.token
  },

  actions: {
    async signup(nome: string, email: string, senha: string) {
      const response = await axios.post('http://localhost:5024/usuarios', {
        nome,
        email,
        senha
      })

      this.user = response.data
    },

    async login(email: string, senha: string) {
      const response = await axios.post('http://localhost:5024/usuarios/login', {
        email,
        senha: senha
      })

      this.token = response.data.token
      this.user = response.data.usuario
      axios.defaults.headers.common['Authorization'] = `Bearer ${this.token}`
    },

    logout() {
      this.user = null
      this.token = null
      delete axios.defaults.headers.common['Authorization']
    }
  }
})
