<script setup>
import { ref } from 'vue'
import { triggerMotorer } from '../services/weatherApi.js'

const starter = ref(false)
const status = ref('')
const fejl = ref(false)

async function startMotorer() {
  starter.value = true
  status.value = ''
  fejl.value = false
  try {
    await triggerMotorer()
    status.value = 'Motorerne er aktiveret!'
  } catch (err) {
    status.value = err.message || 'Noget gik galt.'
    fejl.value = true
  } finally {
    starter.value = false
  }
}
</script>

<template>
  <div class="card" id="motor-styring">
    <h2>Motor styring</h2>
    <button class="motor-btn" @click="startMotorer" :disabled="starter">
      {{ starter ? 'Sender signal…' : 'Start motorer' }}
    </button>
    <p v-if="status" class="motor-status" :class="{ 'motor-fejl': fejl }">
      {{ status }}
    </p>
  </div>
</template>

<style scoped>
.motor-btn {
  width: 100%;
  padding: 14px;
  font-size: 1rem;
  font-weight: 600;
  background: #1a6da8;
  color: white;
  border: none;
  border-radius: 10px;
  cursor: pointer;
  transition: background 0.2s;
}
.motor-btn:hover:not(:disabled) { background: #155d8f; }
.motor-btn:disabled { background: #90b8d4; cursor: default; }
.motor-status { margin-top: 12px; font-size: 0.9rem; color: #1a6da8; }
.motor-fejl { color: #c0392b; }
</style>
