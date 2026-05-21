<script setup>
defineProps({
  size: { type: Number, default: 32 }
})
</script>

<template>
  <svg :width="size" :height="size" viewBox="0 0 64 64" xmlns="http://www.w3.org/2000/svg">
    <path d="M22 8 L14 12 L8 22 L14 26 L18 22 L18 56 L46 56 L46 22 L50 26 L56 22 L50 12 L42 8 L40 12 Q36 16 32 16 Q28 16 24 12 Z"
          fill="#5fa8e8" stroke="#1a4d7a" stroke-width="2" stroke-linejoin="round"/>
    <path d="M24 8 Q28 14 32 14 Q36 14 40 8" fill="none" stroke="#1a4d7a" stroke-width="2" stroke-linecap="round"/>
  </svg>
</template>
