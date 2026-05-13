import pluginVue from 'eslint-plugin-vue'
import {
  defineConfigWithVueTs,
  vueTsConfigs
} from '@vue/eslint-config-typescript'
import eslintConfigPrettier from 'eslint-config-prettier'
import prettierPlugin from 'eslint-plugin-prettier'

export default defineConfigWithVueTs(
  // Глобальные игноры
  {
    ignores: ['node_modules', 'dist', '*.d.ts']
  },

  // Базовая конфигурация для Vue 3
  ...pluginVue.configs['flat/essential'],

  // Рекомендуемые правила TypeScript (без проверки типов)
  vueTsConfigs.recommended,

  // Отключаем правила ESLint, конфликтующие с Prettier
  eslintConfigPrettier,

  // Дополнительные кастомные правила
  {
    plugins: {
      prettier: prettierPlugin
    },
    rules: {
      'prettier/prettier': 'error',
      'vue/multi-word-component-names': 'off'
    }
  }
)
