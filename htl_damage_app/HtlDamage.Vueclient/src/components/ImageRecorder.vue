<template>
    <div class="imageRecorder">
        <input type="file" accept="image/png, image/jpeg" v-on:change="handleFileChange($event)" />
    </div>
</template>
<script>
export default {
    setup() {},
    methods: {
        // https://blog.openreplay.com/building-a-custom-file-upload-component-for-vue/
        handleFileChange(e) {
            // Check if file is selected
            if (e.target.files && e.target.files[0]) {
                // Get uploaded file infos
                const file = e.target.files[0];
                // const fileSize = Math.round((file.size / 1024 / 1024) * 100) / 100;
                // const fileExtention = file.name.split('.').pop();
                // const fileName = file.name.split('.').shift();
                let reader = new FileReader();
                reader.addEventListener('load', () => this.$emit('recorded', reader.result));
                // https://developer.mozilla.org/en-US/docs/Web/API/FileReader/readAsDataURL
                reader.readAsDataURL(file);
            }
        },
    },
};
</script>