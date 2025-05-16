using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehaviours/Playerbot")]
public class Playerbot : AIBehaviour
{
    private Transform alvoAtual;                 // Alvo atual (orb mais próximo)
    private SpriteRenderer renderizadorSprite;  // Componente SpriteRenderer do Playerbot
    private Color corOriginal;                  // Cor original do Playerbot

    public float intervaloBuscaAlvo = 0.5f;     // Intervalo para buscar novo orb
    private float temporizadorBusca = 0f;

    public float distanciaColetaOrb = 0.3f;     // Distância para coletar orb

    // Inicializa componentes e salva valores originais
    public override void Init(GameObject dono, SnakeMovement movimentoDono)
    {
        base.Init(dono, movimentoDono);
        renderizadorSprite = owner.GetComponent<SpriteRenderer>();
        if (renderizadorSprite != null)
        {
            corOriginal = renderizadorSprite.color;
        }
    }

    // Executa a lógica de busca e movimentação para orbs
    public override void Execute()
    {
        // Atualiza temporizador para buscar novo orb
        temporizadorBusca -= Time.deltaTime;
        if (temporizadorBusca <= 0f)
        {
            BuscarOrbMaisProximo();
            temporizadorBusca = intervaloBuscaAlvo;
        }

        // Se existir um orb como alvo, move em direção a ele
        if (alvoAtual != null)
        {
            MoverAte(alvoAtual.position);

            // Verifica se está perto o suficiente para coletar
            float distancia = Vector2.Distance(owner.transform.position, alvoAtual.position);
            if (distancia < distanciaColetaOrb)
            {
                GameObject orb = alvoAtual.gameObject;
                alvoAtual = null; // Limpa referência para evitar erros

                orb.SetActive(false); // Desativa orb imediatamente para evitar conflitos
                ownerMovement.StartCoroutine(DestruirOrbComAtraso(orb, 1f)); // Destroi orb depois de 1 segundo

                AtivarEfeitoVisual(); // Aplica efeito visual simples (cor laranja)
            }
        }
    }

    // Move o Playerbot em direção a uma posição alvo
    void MoverAte(Vector3 posicaoAlvo)
    {
        Vector3 direcao = posicaoAlvo - owner.transform.position;
        direcao.z = 0.0f;

        float angulo = Mathf.Atan2(direcao.x, direcao.y) * Mathf.Rad2Deg;
        Quaternion rotacao = Quaternion.AngleAxis(-angulo, Vector3.forward);
        owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, rotacao, ownerMovement.speed * Time.deltaTime);

        owner.transform.position = Vector2.MoveTowards(owner.transform.position, posicaoAlvo, ownerMovement.speed * Time.deltaTime);
    }

    // Encontra o orb mais próximo na cena
    void BuscarOrbMaisProximo()
    {
        GameObject[] orbs = GameObject.FindGameObjectsWithTag("Orb");
        float menorDistancia = Mathf.Infinity;
        Transform orbMaisProximo = null;

        foreach (GameObject orb in orbs)
        {
            float distancia = Vector2.Distance(owner.transform.position, orb.transform.position);
            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                orbMaisProximo = orb.transform;
            }
        }

        alvoAtual = orbMaisProximo;
    }

    // Inicia o efeito visual de mudança de cor para laranja temporariamente
    void AtivarEfeitoVisual()
    {
        if (renderizadorSprite != null)
        {
            ownerMovement.StartCoroutine(EfeitoVisualTemporario());
        }
    }

    // Coroutine que deixa o Playerbot laranja por 0.15s e depois volta à cor original
    IEnumerator EfeitoVisualTemporario()
    {
        renderizadorSprite.color = new Color(1f, 0.3f, 0f); // cor laranja

        yield return new WaitForSeconds(0.15f);

        renderizadorSprite.color = corOriginal; // volta à cor original
    }

    // Coroutine que destrói o orb após um pequeno atraso
    IEnumerator DestruirOrbComAtraso(GameObject orb, float atraso)
    {
        yield return new WaitForSeconds(atraso);
        if (orb != null)
        {
            Destroy(orb);
        }
    }
}
