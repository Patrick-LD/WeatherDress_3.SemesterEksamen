<script setup>
defineProps({
  text: { type: String, required: true }
})
</script>

<template>
  <div class="card" id="meddelelse-om-vejr">
    <h2>Meddelelse omkring vejr</h2>
    <p>{{ text }}</p>
  </div>
</template>
