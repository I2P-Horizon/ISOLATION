/*
                        PlayerDataDTO

            - �÷��̾� ������ DTO Ŭ����
                - Status (�ɷ�ġ) ������
                - Position (��ġ) ������
*/
[System.Serializable]
public class PlayerDataDTO
{
    public StatusDTO Status;
    public PositionDTO Position;

    [System.Serializable]
    // ���� ����
    public class StatusDTO
    {
        public float maxHp;             // �ִ�ü��
        public float curHp;             // ����ü��
        public float speed;             // �̵��ӵ�
        public float rotateSpeed;       // ȸ���ӵ�
        public float damage;            // �⺻ ���ݷ�
        public float defense;           // �⺻ ����
        public int gold;                // ���� ���
    }

    [System.Serializable]
    // ��ġ ����
    public class PositionDTO
    {
        public float posX;              
        public float posY;
        public float posZ;
    }
}
