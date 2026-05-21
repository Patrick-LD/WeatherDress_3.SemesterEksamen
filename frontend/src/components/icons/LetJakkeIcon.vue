<script setup>
defineProps({
  size: { type: Number, default: 32 }
})
</script>

<template>
  <svg :width="size" :height="size" viewBox="0 0 64 64" xmlns="http://www.w3.org/2000/svg">
    <path d="M20 8 L8 14 L4 36 L14 38 L16 30 L16 58 L48 58 L48 30 L50 38 L60 36 L56 14 L44 8 L38 8 L32 14 L26 8 Z"
          fill="#8fc4a8" stroke="#2d6a4a" stroke-width="2" stroke-linejoin="round"/>
    <line x1="32" y1="14" x2="32" y2="58" stroke="#2d6a4a" stroke-width="1.8" stroke-dasharray="2 2"/>
    <path d="M26 8 L32 14 L38 8" fill="none" stroke="#2d6a4a" stroke-width="2"/>
    <rect x="20" y="40" width="8" height="6" fill="none" stroke="#2d6a4a" stroke-width="1.5" rx="1"/>
    <rect x="36" y="40" width="8" height="6" fill="none" stroke="#2d6a4a" stroke-width="1.5" rx="1"/>
  </svg>
</template>
