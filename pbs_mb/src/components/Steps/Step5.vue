<template>
  <div>
    <group v-if="~this.isAdd" v-for='oriFile in oriFiles'>
      <x-img v-bind:src="onGetFile(oriFile.file_url)" class="preview"></x-img>
      <cell v-bind:title="onRenderFileType(oriFile.file_type)" v-bind:value="oriFile.file_name"></cell>
    </group>
    <group>
      <vue-file-upload v-bind:url="renderUrl()" ref="vueFileUploader" v-bind:filters = "filters"
      v-bind:events = 'cbEvents' v-bind:request-options = "reqopts" v-on:onAdd = "onAddFile" class='uploadbutton' ></vue-file-upload>
    </group>
    <x-hr></x-hr>
    <div v-for='file in files'>
      <group>
        <x-img v-bind:src="onPreView(file)" class="preview"></x-img>
        <x-progress v-bind:percent='file.progress' :show-cancel="false"></x-progress>
        <cell title="传送状态" v-bind:value="onStatus(file)"></cell>
        <XButton type='primary' value='delete' @click.native="deleteItem(file)" v-bind:disabled="onBtnStatus(file)" mini>删除</XButton>
      </group>
    </div>
    <x-hr></x-hr>
    <Group>
      <x-button type="primary" @click.native="onUploadFiles" :disabled="btnstate" >上传全部</x-button>
    </Group>
  </div>
</template>

<script>
import { Group, XInput, XSwitch, PopupPicker, XHr, XButton, Flexbox, FlexboxItem, XProgress, Cell, XImg } from 'vux'
import VueFileUpload from 'vue-file-upload'

export default {
  components: {
    Group,
    XInput,
    PopupPicker,
    VueFileUpload,
    XHr,
    XButton,
    Flexbox,
    FlexboxItem,
    XProgress,
    XImg,
    Cell,
    XSwitch
  },
  mounted: function () {
    this.$parent.step1 = 4
    this.isAdd = this.$route.query.isAdd
    if (~(this.isAdd)) {
      this.$vux.loading.show({
        text: '读取文件列表'
      })
      this.getOriFiles()
    }
  },
  methods: {
    renderUrl () {
      return this.$store.getters.GetterBaseUrl + 'api/Files/singleUpload'
    },
    onStatus (file) {
      if (file.isSuccess) {
        return '上传成功'
      } else if (file.isError) {
        return '上传失败'
      } else if (file.isUploading) {
        return '正在上传'
      } else {
        return '待上传'
      }
    },
    getOriFiles () {
      let uuid = this.$store.getters.GetterEntity.uuid
      this.$http({
        url: this.$store.getters.GetterBaseUrl + 'api/Files/GetFileList',
        method: 'GET',
        emulateJSON: true,
        params: {
          uuid: uuid
        },
        headers: {
          contentType: 'application/x-www-form-urlencoded',
          token: this.$store.getters.GetterToken
        }
      }).then(function (res) {
        if (res.data.code === 100) {
          this.oriFiles = res.data.resultList
          this.$vux.loading.hide()
        } else {
          this.$vux.loading.hide()
          this.$vux.toast.show({
            text: res.data.message,
            type: 'warn'
          })
        }
      }).catch(err => {
        this.$vux.loading.hide()
        this.$vux.toast.show({
          text: err,
          type: 'warn'
        })
      })
    },
    onGetFile (fileUrl) {
      let picUrl = this.$store.getters.GetterBaseUrl + 'api/GetFile?fileName=' + fileUrl
      return picUrl
    },
    onRenderFileType (type) {
      switch (type) {
        case 2001:
          return '现场图'
        case 2002:
          return '方位图'
        case 2003:
          return '见证人签名图'
        case 2004:
          return '草图'
        default:
          return '其他'
      }
    },
    onBtnStatus (file) {
      if (file.isSuccess) {
        return true
      } else if (file.isUploading) {
        return true
      } else {
        return false
      }
    },
    onPreView (file) {
      let src = window.URL.createObjectURL(file.file)
      return src
    },
    onChooseFile (files) {
      this.$refs.vueFileUploader.click()
    },
    onAddFile (files) {
      this.files = files
    },
    uploadItem (file) {
      // 单个文件上传
      file.upload()
    },
    deleteItem (file) {
      file.remove()
    },
    onUploadFiles () {
      // 上传所有文件
      this.$refs.vueFileUploader.uploadAll()
    }
  },
  data () {
    return {
      isAdd: true,
      btnstate: false,
      completeNames: [],
      oriFiles: [],
      files: [],
      // 文件过滤器，只能上传图片
      filters: [
        {
          name: 'imageFilter',
          fn (file) {
            var type = '|' + file.type.slice(file.type.lastIndexOf('/') + 1) + '|'
            return '|jpg|png|jpeg|bmp|gif|'.indexOf(type) !== -1
          }
        }
      ],
      cbEvents: {
        onCompleteUpload: (file, response, status, header) => {
          if (status === 200) {
            this.completeNames.push(response.file)
          }
        },
        onAddFileSuccess: (file) => {
        }
      },
      // xhr请求附带参数
      reqopts: {
        headers: {
          token: this.$store.getters.GetterToken
          // token: '6a8a77de-4e2d-449b-9b98-7d4e7b4fab5a'
        },
        responseType: 'json',
        withCredentials: false
      }
    }
  }
}
</script>

<style>
  .uploadbutton{
  }
  .preview {
    width: 200px;
    height: 200px
  }
</style>
