interface ITokenDto{
    valid: boolean;
}

class TokenDto implements ITokenDto {
    valid: boolean;
    
    constructor(valid: boolean) {
        this.valid = valid;
    }
}

export default TokenDto;