interface IBasicResultDto<T> {
    amount: number;
    resultDto: T[];
}

interface IBasicDto<T>{
    isSuccess: boolean;
    error: string;
    value: IBasicResultDto<T>;
}

class BasicDto<T> implements IBasicDto<T> {
    isSuccess: boolean;
    error: string;
    value: IBasicResultDto<T>;
    
    constructor(isSuccess: boolean, error: string, value: IBasicResultDto<T>) {
        this.isSuccess = isSuccess;
        this.error = error;
        this.value = value;
    }
}