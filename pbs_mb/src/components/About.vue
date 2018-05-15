<template>
  <div>
    <x-header title="关于深圳E勘查" :left-options="{showBack: true}" @on-click-more="showMenu = true" 
      :right-options="{showMore: true}">
      <img slot="overwrite-right" src="../../static/css/images/icons/navicon/icon-refresh.png" size="35" style="fill:#fff;position:relative;top:-8px;left:-3px;" />
    </x-header>

    <div class="content" style="margin-top:20px">
        <div class="iscroll-wrapper">
            <div>
                <div class="container-fluid">
                    <div class="row-fluid">
                        
                        <div class="span9" style="line-height: 0.7;">
                            <h2>深圳E勘查</h2>
                            <br>
                            <p>
                              版本：<span class="normal-text">v1.00</span>
                            </p>
                            <br>
                            <p>发布日期：<span class="normal-text">2018-05-15</span></p>
                        </div>
                    </div>
                    <br>
                    <div style="height: 1px;background-color: gainsboro;"></div>
                </div>

                <div class="container-fluid">
                    <div class="row-box">
                        <div class="span1" style="line-height: 1.2">
                        <br>
                            <h2>介绍</h2>
                            <br>
                            <p class="normal-text">深圳E勘查是简易现场勘查软件</p>
                        </div>
                    </div>
                    <br>
                    <div class="row-box" style="margin-top: 10px;">
                        <div class="span1" style="line-height: 1.2">
                            <span style="font-weight: bold;">建设单位：</span>
                            <span class="normal-text">深圳市公安局南山分局</span>
                        </div>
                    </div>

                    <div class="row-box" style="margin-top: 10px;">
                        <div class="span1" style="line-height: 1.2">
                            <span style="font-weight: bold;">开发单位：</span>
                            <span class="normal-text">深圳朗坤科技开发有限公司</span>
                        </div>
                    </div>

                    <div style="height: 1px;background-color: gainsboro;margin: 0px 0;"></div>
                </div>
                <br>

                <div class="container-fluid">
                    <div class="row-box">
                        <div class="span1" style="line-height: 1.0">
                        <br>
                            <h2>当前版本</h2>
                            <br>
                            <p class="normal-text">版本：v1.00</p>
                            <br>
                            <p class="normal-text">发布日期：2018-05-15</p>
                            <br>
                            <p class="normal-text">1.首次上线。</p>
                        </div>
                    </div>
                    <div style="height: 1px;background-color: gainsboro;margin-top: 0px;"></div>
                </div>
                <br>
                <div class="container-fluid" >
                    <div class="row-box">
                        <div class="span1" style="line-height: 1.0">
                        <br>
                            <h2>历史版本</h2>
                            <br>
                            <!-- 下面为示例，从本地更新拷贝之前的版本即可 最新的放在最上面-->
                            <p class="normal-text">暂无</p>
                            
                        </div>
                    </div>

                </div>
            </div>
        </div>

    </div>
  </div>
</template>

<script>
import { XHeader } from 'vux'
import { CLR_RECORDINFO } from '../mutationTypes'
export default {
  components: {
    XHeader
  },
  computed: {
    
  },
  mounted: function () {
    
  },
  methods: {
  },
  data () {
    return {
    }
  }
}
</script>

<style>
  
</style>
