<script setup>
import axios from 'axios';
</script>

<template>
    <nav>
        <ul class="nav_links">
            <router-link to="/">
                <li>Home</li>
            </router-link>            
            <router-link to="/newdamage">
                <li>Neuer Schaden</li>
            </router-link>
            <router-link to="/login" v-if="!authenticated">
                <li>
                    Login
                </li>
            </router-link>
            <a href="" v-on:click="logout()"
                ><li v-if="authenticated">Logout {{ userdata.username }}</li></a
            >
        </ul>
    </nav>
</template>


<style scoped>
header .logo {
    width: 220px;
    cursor: pointer;
}

.nav_links {
    margin-right: 100px;
}

.nav_links li {
    display: inline-block;
    padding: 0px 10px;
}

.nav_links li a {
    transition: all 0.3s ease 0s;
}

.nav_links li:hover {
    color: rgba(0, 185, 232, 0.8);
    text-decoration: none;
}
</style>

<script>
export default {
    setup() {},
    computed: {
        authenticated() {
            return this.$store.state.userdata.username ? true : false;
        },
        userdata() {
            return this.$store.state.userdata;
        },
    },
    methods: {
        logout() {
            delete axios.defaults.headers.common['Authorization'];
            this.$store.commit('authenticate', null);
        },
    },
};
</script>