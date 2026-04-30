<script setup>
defineProps({
  items: { type: Array, required: true }
})
</script>

<template>
  <div class="card" id="toj-anbefalet">
    <h2>Tøj anbefalet</h2>
    <div class="clothing-list">
      <span v-for="item in items" :key="item" class="clothing-item">{{ item }}</span>
    </div>
  </div>
</template>
