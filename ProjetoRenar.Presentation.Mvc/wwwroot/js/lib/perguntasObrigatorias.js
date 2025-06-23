function validarObrigatoria(botao, action) {
    //Recupera o form do botão
    let formulario = $(botao).parents("form");

    let valido = true;
    let primeiraPerguntaSemResposta = null;

    //Recupera as perguntas obrigatórias dentro desse 
    let perguntasObrigatorias = $(formulario).find("div.obrigatoria");
    perguntasObrigatorias.each(function (index)
    {
        let tipoControle = $(this).attr("tipocontrole");
        let preenchido = true;
        switch (tipoControle) {
            case "1":
                preenchido = $(this).children("input[type=radio]:checked").length > 0;
                break;

            case "2":
                preenchido = $(this).children("input[type=checkbox]:checked").length > 0;
                break;

            case "4":
                preenchido = !!$(this).children("textarea").val();
                break;
        }

        if (!preenchido) {
            if (!primeiraPerguntaSemResposta) primeiraPerguntaSemResposta = $(this);
            valido = valido && false;
        }
    });

    if (!valido) {

        $('html').animate({ scrollTop: 80 }, 'slow');
        //alert('Para finalizar a avaliação é necessário responder a todas as perguntas obrigatórias');
        $(".mensagem").show();
        return false;
    } else {
        formulario.attr("action", action);
        formulario.submit();
        return true;
    }
}
