import Hello from 'components/HelloFromVux'
import Test from 'components/Test'
import EditRecordBasic from 'components/Steps/EditRecordBasic'
import Step1 from 'components/Steps/Step1'
import Step2 from 'components/Steps/Step2'
import Step3 from 'components/Steps/Step3'
import Step4 from 'components/Steps/Step4'
import Step5 from 'components/Steps/Step5'
import OnlineDraw from 'components/OnlineDraw'
import MainIndex from 'components/MainIndex'
import RecordList from 'components/RecordList'
import FetchMapLoc from 'components/Steps/FetchMapLoc'

export default [
  {
    path: '/',
    name: 'Hello',
    component: Hello
  },
  {
    path: '/Test',
    name: 'Test',
    component: Test
  },
  {
    path: '/OnlineDraw',
    name: 'OnlineDraw',
    component: OnlineDraw
  },
  {
    path: '/MainIndex',
    name: 'MainIndex',
    component: MainIndex
  },
  {
    path: '/RecordList',
    name: 'RecordList',
    component: RecordList
  },
  {
    path: '/FetchMapLoc',
    name: 'FetchMapLoc',
    component: FetchMapLoc
  },
  {
    path: '/EditRecordBasic',
    component: EditRecordBasic,
    children: [
      {path: '', component: Step1, name: 'Step1'},
      { path: 'Step1', component: Step1 },
      { path: 'Step2', component: Step2, name: 'Step2' },
      { path: 'Step3', component: Step3, name: 'Step3' },
      { path: 'Step4', component: Step4, name: 'Step4' },
      { path: 'Step5', component: Step5, name: 'Step5' }
    ]
  }
]
