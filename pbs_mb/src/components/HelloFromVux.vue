<template>
  <div>
    <div class="vux-demo">
      <img class="logo" src="../assets/gongan.png">
      <h3>深圳E勘查</h3>
    </div>
    <group>
      <x-input title="用户名" placeholder="请输入用户名" novalidate :icon-type="iconType" :show-clear="false" v-model="loginName" ></x-input>
      <x-input title="密码 " placeholder="请输入密码" novalidate :icon-type="iconType" :show-clear="false" v-model='password'
      type="password" ></x-input>
      <x-button type="primary" @click.native="login" :disabled="disable001" >登录</x-button>
      <a href="http://182.61.43.29/webapi/api/Version/GetNewestApk">下载app</a>
    </group>
  </div>
</template>

<script>
import { Group, XInput, XButton } from 'vux'
import { SET_BASEINFO, SET_SYSINFO } from '../mutationTypes'

var md5 = require('js-md5')

export default {
  components: {
    Group,
    XButton,
    XInput
  },
  mounted: function () {
    let url = 'http://182.61.43.29/webapi/'
    //url = 'http://localhost/PbsApi/'
    this.$store.commit(SET_SYSINFO, url)
  },
  methods: {
    login () {
      this.disable001 = true
      this.$vux.loading.show({
        text: '登陆中'
      })
      var loginInfo = 'username=' + this.loginName + '&password=' + md5(this.password)
      this.$http({
        url: this.$store.getters.GetterBaseUrl + 'Auth/Login?' + loginInfo,
        method: 'Get',
        emulateJSON: true,
        headers: {
          contentType: 'application/x-www-form-urlencoded'
        }
      }).then(function (res) {
        // let data = JSON.parse(res.data)
        if (res.data.code === 100) {
          this.$store.commit(SET_BASEINFO, res.data)
          this.$vux.loading.hide()
          this.success()
        } else {
          this.$vux.loading.hide()
          this.$vux.toast.show({
            text: res.data.message,
            type: 'warn'
          })
          this.disable001 = false
        }
      }).catch(err => {
        this.$vux.loading.hide()
        this.$vux.toast.show({
          text: err,
          type: 'warn'
        })
        this.disable001 = false
      })
    },
    success () {
      this.$store.dispatch('clearList')
      let params = {token: this.$store.getters.GetterToken, baseUrl: this.$store.getters.GetterBaseUrl, myUnitId: this.$store.getters.GetterMe.member.unit_id}
      this.$store.dispatch('fetchUserList', params)
      this.$store.dispatch('fetchUnitList', params)
      this.$router.push({path: '/MainIndex'})
    }
  },
  data () {
    return {
      iconType: '',
      disable001: false,
      loginName: '',
      password: ''
    }
  }
}
</script>

<style>
.vux-demo {
  text-align: center;
}
.logo {
  width: 100px;
  height: 100px
}
</style>
