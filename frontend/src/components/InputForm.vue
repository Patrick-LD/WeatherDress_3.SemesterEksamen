<script setup>
defineProps({
  modelValue: { type: String, required: true },
  loading: { type: Boolean, default: false },
  fejl: { type: String, default: '' }
})

const emit = defineEmits(['update:modelValue', 'submit'])

function onInput(e) {
  emit('update:modelValue', e.target.value)
}
</script>

<template>
  <div id="input-section">
    <h1>WeatherDress</h1>
    <p>Indtast dit postnummer for at få vejr og påklædningsforslag</p>
    <input
      id="postnummer-input"
      :value="modelValue"
      @input="onInput"
      @keydown.enter="emit('submit')"
      type="text"
      placeholder="Postnummer"
      maxlength="4"
      inputmode="numeric"
    />
    <button id="submit-button" @click="emit('submit')" :disabled="loading">
      {{ loading ? 'Henter vejr…' : 'Søg vejr' }}
    </button>
    <div v-if="fejl" id="error-message">{{ fejl }}</div>
  </div>
</template>
