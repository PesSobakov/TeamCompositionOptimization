import { Subscription } from "./subscription";

export interface AccountInfo
{
  login: string;
  subscription: Subscription;
  subscriptionDueDate: Date;
  isSuperuser: boolean;
}
