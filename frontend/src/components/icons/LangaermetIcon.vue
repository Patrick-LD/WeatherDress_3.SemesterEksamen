<script setup>
defineProps({
  size: { type: Number, default: 32 }
})
</script>

<template>
  <svg :width="size" :height="size" viewBox="0 0 64 64" xmlns="http://www.w3.org/2000/svg">
    <path d="M22 8 L8 14 L4 38 L14 40 L16 28 L16 56 L48 56 L48 28 L50 40 L60 38 L56 14 L42 8 L40 12 Q36 16 32 16 Q28 16 24 12 Z"
          fill="#7fb6d8" stroke="#1a4d7a" stroke-width="2" stroke-linejoin="round"/>
    <path d="M24 8 Q28 14 32 14 Q36 14 40 8" fill="none" stroke="#1a4d7a" stroke-width="2" stroke-linecap="round"/>
    <path d="M14 40 Q12 42 12 44 M50 40 Q52 42 52 44" stroke="#1a4d7a" stroke-width="1.5" fill="none"/>
  </svg>
</template>
