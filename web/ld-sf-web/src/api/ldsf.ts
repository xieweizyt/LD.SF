import { backboneService } from './request'

const baseUrl = '/api/ldsf'

export interface LoginInput {
  userName: string
  password: string
}

export interface LoginDto {
  userId: string
  userName: string
  displayName: string
  role: string
}

export interface SubaccountDto {
  id: string
  publicId: string
  name: string
  balance: number
  sentCount: number
  authorizationIdentifier?: string
  totalGranted: number
  usedCount: number
  creationTime: string
}

export interface SmsTaskDto {
  id: string
  subaccountId: string
  taskName: string
  totalNumbers: number
  batchSize: number
  content1: string
  content2?: string
  sentCount: number
  status: string
  creationTime: string
}

export interface UsageLedgerDto {
  id: string
  subaccountId: string
  identifier: string
  type: string
  count: number
  taskId?: string
  note: string
  creationTime: string
}

export function login(data: LoginInput): Promise<LoginDto> {
  return backboneService.post(`${baseUrl}/auth/login`, data)
}

export function getSubaccounts(): Promise<SubaccountDto[]> {
  return backboneService.get(`${baseUrl}/admin/subaccounts`)
}

export function createSubaccount(data: {
  name: string
  initialBalance: number
  identifier?: string
}): Promise<SubaccountDto> {
  return backboneService.post(`${baseUrl}/admin/subaccounts`, data)
}

export function grantUses(identifier: string, data: { uses: number; reason?: string }): Promise<SubaccountDto> {
  return backboneService.post(`${baseUrl}/admin/authorizations/${identifier}/grant`, data)
}

export function getLedgers(identifier?: string): Promise<UsageLedgerDto[]> {
  return backboneService.get(`${baseUrl}/admin/ledger`, { params: { identifier } })
}

export function getTasks(subaccountId: string): Promise<SmsTaskDto[]> {
  return backboneService.get(`${baseUrl}/subaccounts/${subaccountId}/tasks`)
}

export function createTask(
  subaccountId: string,
  data: {
    taskName: string
    phoneNumbers: string[]
    batchSize: number
    content1: string
    content2?: string
  },
): Promise<SmsTaskDto> {
  return backboneService.post(`${baseUrl}/subaccounts/${subaccountId}/tasks`, data)
}

export function deleteTask(subaccountId: string, taskId: string): Promise<void> {
  return backboneService.delete(`${baseUrl}/subaccounts/${subaccountId}/tasks/${taskId}`)
}

