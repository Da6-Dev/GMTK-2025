using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // A referência para o Transform do nosso jogador (o carrinho)
    public Transform target;

    // A velocidade com que a câmera vai seguir o jogador
    public float smoothSpeed = 0.125f;

    // Offset 2D, caso você queira que a câmera fique um pouco deslocada do centro do jogador (ex: para ver mais à frente)
    // Para um seguimento centralizado, deixe em (0, 0).
    public Vector2 offset;
    
    // A posição Z fixa da câmera. -10 é o padrão para ver a cena 2D.
    private float cameraZPosition = -10f;
    
    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        // Garante que temos um alvo para seguir
        if (target == null)
        {
            return;
        }

        // A posição desejada agora é a posição X e Y do alvo (+ offset), mas com o Z da câmera fixo.
        Vector3 desiredPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, cameraZPosition);
        
        // A função de suavização continua a mesma, mas agora respeitando o Z fixo.
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
    }
}