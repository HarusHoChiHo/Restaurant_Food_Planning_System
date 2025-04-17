class LoginDto<T> implements IBasicDto<T> {
    isSuccess: boolean;
    error: string;
    value: IBasicResultDto<T>;
    token: string;

    constructor(isSuccess: boolean, error: string, value: IBasicResultDto<T>, token: string) {
        this.isSuccess = isSuccess;
        this.error = error;
        this.value = value;
        this.token = token;
    }
    
}