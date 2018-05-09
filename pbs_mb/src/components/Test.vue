<template>
  <div>
    <x-header :left-options="{showBack: true, backText: '返回'}">查看最近7天的记录</x-header>
    <br>
    <div>
       
      <x-button type="primary" @click.native="test"  >test</x-button>
    </div>
  </div>
</template>

<script>
  import { XHeader, XButton } from 'vux'
  //import Cordova from '../js/cordova.js'

  export default {
    components: {      
      XHeader,
      XButton
    },
    methods: {
      test () {
        Cordova.exec(this.onSuccess, this.onError, 'Interactive', 'shotMapInfo', [])
      },
      onSuccess: function (result) {
        alert('ok:' + result)
      },
      onError: function (result) {
        alert('error' + result)
      }
    },
    data () {
      return {
        recList: [],
        status: {
          pulldownStatus: 'default'
        }
      }
    }
  }
</script>