function formata(campo, mask, evt) {

    if (document.all) { // Internet Explorer
        key = evt.keyCode;
    }
    else { // Nestcape
        key = evt.which;
    }

    string = campo.value;
    i = string.length;

    if (i < mask.length) {
        if (mask.charAt(i) == '§') {
            return (key > 47 && key < 58);
        } else {
            if (mask.charAt(i) == '!') { return true; }
            for (c = i; c < mask.length; c++) {
                if (mask.charAt(c) != '§' && mask.charAt(c) != '!')
                    campo.value = campo.value + mask.charAt(c);
                else if (mask.charAt(c) == '!') {
                    return true;
                } else {
                    return (key > 47 && key < 58);
                }
            }
        }
    } else return false;
}

function isNumeric(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

function countOccurrence(palavra,char) {
    var count = 0;
    for (var i = 0; i < palavra.length; i++) {
        if (palavra[i] === char) {
            count++;
        }
    }
    return count;
}

function Valida_Numero_Processo(_numero) {
    if (_numero == "") {
        return "Digite o número do processo!";
    }

    if (_numero.length < 8) {
        return "Número do processo inválido. Utilize o formato '####0-0/0000' (onde # é opcional)";
    }

    var _count = countOccurrence(_numero, '-');
    var _count2 = countOccurrence(_numero, '/');
    if (_count != 1 || _count2 != 1) {
        return "Número do processo inválido. Utilize o formato '####0-0/0000' (onde # é opcional)";
    }

    var ano = _numero.slice(_numero.length - 4);
    
    
    if (ano < 1950 || ano > new Date().getFullYear()) {
        return "Processo não cadastrado.";
    }

    var barra = _numero.slice(_numero.length - 5);
    if (barra.substring(0, 1) != "/") {
        return "Número do processo inválido. Utilize o formato '####0-0/0000' (onde # é opcional)";
    }


    var _numero2 = _numero.substring(_numero.lastIndexOf("-") + 1, _numero.lastIndexOf("/"));
    if (!isNumeric(_numero2) || _numero2 > 9) {
        return "Número do processo inválido. Utilize o formato '####0-0/0000' (onde # é opcional)";
    }

    var _pos = _numero.length - 7;
    if (!isNumeric(_numero.substring(0, _pos))) {
        return "Número do processo inválido. Utilize o formato '####0-0/0000' (onde # é opcional)";
    }

    
    return "";
}

function valida_Cpf(cpf) {
    cpf = cpf.split("").filter(n => (Number(n) || n == 0)).join("");
    var numeros, digitos, soma, i, resultado, digitos_iguais;
    digitos_iguais = 1;
    if (cpf.length < 11)
        return false;
    for (i = 0; i < cpf.length - 1; i++)
        if (cpf.charAt(i) != cpf.charAt(i + 1)) {
            digitos_iguais = 0;
            break;
        }
    if (!digitos_iguais) {
        numeros = cpf.substring(0, 9);
        digitos = cpf.substring(9);
        soma = 0;
        for (i = 10; i > 1; i--)
            soma += numeros.charAt(10 - i) * i;
        resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
        if (resultado != digitos.charAt(0))
            return false;
        numeros = cpf.substring(0, 10);
        soma = 0;
        for (i = 11; i > 1; i--)
            soma += numeros.charAt(11 - i) * i;
        resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
        if (resultado != digitos.charAt(1))
            return false;
        return true;
    }
    else
        return false;
}

function valida_Cnpj(cnpj) {

    cnpj = cnpj.replace(/[^\d]+/g, '');

    if (cnpj == '') return false;

    if (cnpj.length != 14)
        return false;

    // Elimina CNPJs invalidos conhecidos
    if (cnpj == "00000000000000" ||
        cnpj == "11111111111111" ||
        cnpj == "22222222222222" ||
        cnpj == "33333333333333" ||
        cnpj == "44444444444444" ||
        cnpj == "55555555555555" ||
        cnpj == "66666666666666" ||
        cnpj == "77777777777777" ||
        cnpj == "88888888888888" ||
        cnpj == "99999999999999")
        return false;

    // Valida DVs
    tamanho = cnpj.length - 2
    numeros = cnpj.substring(0, tamanho);
    digitos = cnpj.substring(tamanho);
    soma = 0;
    pos = tamanho - 7;
    for (i = tamanho; i >= 1; i--) {
        soma += numeros.charAt(tamanho - i) * pos--;
        if (pos < 2)
            pos = 9;
    }
    resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
    if (resultado != digitos.charAt(0))
        return false;

    tamanho = tamanho + 1;
    numeros = cnpj.substring(0, tamanho);
    soma = 0;
    pos = tamanho - 7;
    for (i = tamanho; i >= 1; i--) {
        soma += numeros.charAt(tamanho - i) * pos--;
        if (pos < 2)
            pos = 9;
    }
    resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
    if (resultado != digitos.charAt(1))
        return false;

    return true;

}

function Valida_Data(data) {
    if (data == "") {
        return false;
    }
    var patternValidaData = /^(((0[1-9]|[12][0-9]|3[01])([-.\/])(0[13578]|10|12)([-.\/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([-.\/])(0[469]|11)([-.\/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([-.\/])(02)([-.\/])(\d{4}))|((29)(\.|-|\/)(02)([-.\/])([02468][048]00))|((29)([-.\/])(02)([-.\/])([13579][26]00))|((29)([-.\/])(02)([-.\/])([0-9][0-9][0][48]))|((29)([-.\/])(02)([-.\/])([0-9][0-9][2468][048]))|((29)([-.\/])(02)([-.\/])([0-9][0-9][13579][26])))$/;
    if (!patternValidaData.test(data)) {
        return false;
    }
    return true;
}

function DataMaior(date1, date2) {
    let parts1 = date1.split('/')
    let parts2 = date2.split('/')

    date1 = new Date(parts1[2], parts1[1] - 1, parts1[0]).getTime()
    date2 = new Date(parts2[2], parts2[1] - 1, parts2[0]).getTime()

    return date1 > date2 ? true : false
}

function DataMenor(date1, date2) {
    let parts1 = date1.split('/')
    let parts2 = date2.split('/')

    date1 = new Date(parts1[2], parts1[1] - 1, parts1[0]).getTime()
    date2 = new Date(parts2[2], parts2[1] - 1, parts2[0]).getTime()

    return date1 < date2 ? true : false
}

function DataIgual(date1, date2) {
    let parts1 = date1.split('/')
    let parts2 = date2.split('/')

    date1 = new Date(parts1[2], parts1[1] - 1, parts1[0]).getTime()
    date2 = new Date(parts2[2], parts2[1] - 1, parts2[0]).getTime()

    return date1 == date2 ? true : false
}

function DataIntervalo(date, date1, date2) {
    var retorno = false;

    let parts = date.split('/')
    let parts1 = date1.split('/')
    let parts2 = date2.split('/')

    date = new Date(parts[2], parts[1] - 1, parts[0]).getTime()
    date1 = new Date(parts1[2], parts1[1] - 1, parts1[0]).getTime()
    date2 = new Date(parts2[2], parts2[1] - 1, parts2[0]).getTime()

    if (date >= date1 && date <= date2) {
        return true;
    } else {
        return false;
    }
}

function RetornaAno(date) {
    let parts = date.split('/')
    date = new Date(parts[2], parts[1] - 1, parts[0]).getTime()
    var d = new Date(date)
    var n = d.getFullYear()
    return n
}



