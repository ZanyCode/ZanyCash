export enum PaymentIntervalTypeModel
{
    daily,
    weekly,
    monthly,
    yearly
}

export interface PaymentIntervalModel
{
    interval: number;

    intervalType: PaymentIntervalTypeModel;
}

export interface PaymentAmountModel
{
    date: Date | string;

    amount: number;
}

export interface TransactionModel
{
    isOnetimeTransaction: boolean;

    onetimeTransaction: OnetimeTransactionModel;

    recurringTransaction: RecurringTransactionModel;
}

export interface OnetimeTransactionModel
{
    id: string;

    date: Date | string;

    description: string;

    amount: number;
}

export interface RecurringTransactionModel
{
    id: string;

    endDate: Date | string;

    description: string;

    interval: PaymentIntervalModel;

    amounts: PaymentAmountModel[];
}
