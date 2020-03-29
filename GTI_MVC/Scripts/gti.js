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
