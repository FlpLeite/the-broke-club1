<template>
  <div class="bg-gradient-to-b from-primary-50 to-white dark:from-gray-900 dark:to-gray-800">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16 md:py-24">
      <div class="grid grid-cols-1 md:grid-cols-2 gap-8 items-center">
        <div>
          <h1 class="text-4xl md:text-5xl font-bold text-gray-900 mb-4">
            Assuma o Controle das Suas Finanças
          </h1>
          <p class="text-lg text-gray-600 mb-8">
            O The Broke Club ajuda você a acompanhar suas receitas e despesas, gerenciar seu orçamento e alcançar seus objetivos financeiros.
          </p>
          <div class="flex flex-col sm:flex-row space-y-4 sm:space-y-0 sm:space-x-4">
            <router-link to="/signup" class="btn btn-primary text-center py-3 px-6 text-lg">
              Começar Agora
            </router-link>
            <router-link to="/login" class="btn btn-secondary text-center py-3 px-6 text-lg">
              Entrar
            </router-link>
          </div>
        </div>
        <div class="hidden md:block">
          <div class="flex items-center justify-center">
            <img src="../assets/logoBrokeDark2.png" alt="The Broke Club" class="w-65" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          </div>
        </div>
      </div>
    </div>

    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16">
      <h2 class="text-3xl font-bold text-center text-gray-900 mb-12">Principais Recursos</h2>
      <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-md">
          <div class="text-primary-500 mb-4">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-12 w-12" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 7h6m0 10v-3m-3 3h.01M9 17h.01M9 14h.01M12 14h.01M15 11h.01M12 11h.01M9 11h.01M7 21h10a2 2 0 002-2V5a2 2 0 00-2-2H7a2 2 0 00-2 2v14a2 2 0 002 2z" />
            </svg>
          </div>
          <h3 class="text-xl font-semibold mb-2">Acompanhe Transações</h3>
          <p class="text-gray-600">
            Registre e categorize facilmente suas receitas e despesas para acompanhar para onde seu dinheiro está indo.
          </p>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-md">
          <div class="text-primary-500 mb-4">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-12 w-12" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 3.055A9.001 9.001 0 1020.945 13H11V3.055z" />
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20.488 9H15V3.512A9.025 9.025 0 0120.488 9z" />
            </svg>
          </div>
          <h3 class="text-xl font-semibold mb-2">Visualize Dados </h3>
          <p class="text-gray-600">
            Veja seus dados financeiros em gráficos claros e intuitivos para entender melhor seus hábitos de gastos.
          </p>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-md">
          <div class="text-primary-500 mb-4">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-12 w-12" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>
          <h3 class="text-xl font-semibold mb-2">Atualizações em Tempo Real</h3>
          <p class="text-gray-600">
            Receba atualizações instantâneas sobre seu status financeiro conforme adiciona novas transações e gerencia suas contas.
          </p>
        </div>
      </div>
    </div>

    <div class="bg-primary-600 py-16">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
        <h2 class="text-3xl font-bold text-white mb-4">Pronto para Assumir o Controle das Suas Finanças?</h2>
        <p class="text-xl text-primary-100 mb-8">
          Junte-se a milhares de usuários que já estão gerenciando suas finanças com o The Broke Club.
        </p>
        <router-link to="/signup" class="inline-block btn bg-white dark:bg-gray-800 text-primary-600 dark:text-primary-300 hover:bg-gray-100 dark:hover:bg-gray-700 py-3 px-8 text-lg font-medium">
          Cadastre-se Agora
        </router-link>
      </div>
    </div>
  </div>
</template>