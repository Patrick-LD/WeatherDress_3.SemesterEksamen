<script setup>
defineProps({
  size: { type: Number, default: 64 }
})
</script>

<template>
  <svg :width="size" :height="size" viewBox="0 0 64 64" xmlns="http://www.w3.org/2000/svg">
    <circle cx="22" cy="26" r="10" fill="#7A9DB8"/>
    <circle cx="34" cy="20" r="13" fill="#7A9DB8"/>
    <circle cx="46" cy="26" r="10" fill="#7A9DB8"/>
    <rect x="12" y="26" width="44" height="12" fill="#7A9DB8"/>
    <line x1="22" y1="44" x2="18" y2="58" stroke="#4A90D9" stroke-width="3.5" stroke-linecap="round"/>
    <line x1="34" y1="44" x2="30" y2="58" stroke="#4A90D9" stroke-width="3.5" stroke-linecap="round"/>
    <line x1="46" y1="44" x2="42" y2="58" stroke="#4A90D9" stroke-width="3.5" stroke-linecap="round"/>
  </svg>
</template>
