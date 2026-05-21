<script setup>
defineProps({
  size: { type: Number, default: 32 }
})
</script>

<template>
  <svg :width="size" :height="size" viewBox="0 0 64 64" xmlns="http://www.w3.org/2000/svg">
    <path d="M18 10 Q18 2 32 2 Q46 2 46 10 L46 16 L18 16 Z" fill="#d8e8f8" stroke="#0a1f3a" stroke-width="2" stroke-linejoin="round"/>
    <path d="M14 14 L4 22 L2 42 L12 44 L14 34 L14 62 L50 62 L50 34 L52 44 L62 42 L60 22 L50 14 L46 18 L32 24 L18 18 Z"
          fill="#1a3d5c" stroke="#0a1f3a" stroke-width="2" stroke-linejoin="round"/>
    <line x1="32" y1="24" x2="32" y2="62" stroke="#0a1f3a" stroke-width="2"/>
    <path d="M14 28 L14 34 M14 38 L14 44 M14 48 L14 54 M14 58 L14 62" stroke="#d8e8f8" stroke-width="2"/>
    <path d="M50 28 L50 34 M50 38 L50 44 M50 48 L50 54 M50 58 L50 62" stroke="#d8e8f8" stroke-width="2"/>
    <path d="M18 30 L46 30 M18 38 L46 38 M18 46 L46 46 M18 54 L46 54" stroke="#0a1f3a" stroke-width="0.6" opacity="0.5"/>
  </svg>
</template>
